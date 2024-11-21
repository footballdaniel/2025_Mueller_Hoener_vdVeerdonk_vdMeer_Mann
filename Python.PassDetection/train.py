import glob
import json
import os
import pickle
from dataclasses import asdict
from pathlib import Path

import onnx
import torch
import torch.nn as nn
from matplotlib import pyplot as plt
from torch.utils.data import Subset, DataLoader

from src.domain.inferences import Split
from src.features.feature_engineer import FeatureEngineer
from src.features.foot_offset import FootOffsetCalculator
from src.features.velocities_dominant_foot import VelocitiesDominantFootCalculator
from src.features.velocities_non_dominant_foot import VelocitiesNonDominantFootCalculator
from src.features.zeroed_position_dominant_foot import ZeroedPositionDominantFootCalculator
from src.io.enum_encoder import CustomEncoder
from src.io.raw_data_reader import read_recording_from_json, read_pass_events_from_csv
from src.nn.accuracy import prediction_accuracy
from src.nn.brier import prediction_brier
from src.nn.f1_scores import predict_precision_recall_f1
from src.nn.lstm_model import PassDetectionModel
from src.nn.pass_dataset import PassDataset
from src.services.augmenter import Augmenter
from src.services.label_creator import LabelCreator
from src.services.plotter import plot_sample_with_features

"""Reading Trials"""
pattern = "../Data/Pilot_4/**/*.csv"
plot_dir = Path("plots")
save_plots = False

trials = []
for filename in glob.iglob(pattern, recursive=True):
    print(f"Processing file: {filename}")
    json_filename = filename.replace(".csv", ".json")
    if not os.path.isfile(json_filename):
        print("No JSON file found for", filename)
        continue

    trial = read_recording_from_json(json_filename)
    pass_events = read_pass_events_from_csv(filename)
    trial.pass_events.extend(pass_events)  # Append the events to the Trial
    trials.append(trial)

"""SAMPLING AND AUGMENTATION"""
labeled_samples = LabelCreator.generate(trials)
augmented_samples = Augmenter.augment(labeled_samples, only_augment_passes=True)

"""FEATURE ENGINEERING"""
engineer = FeatureEngineer()
engineer.add_feature(ZeroedPositionDominantFootCalculator())
engineer.add_feature(FootOffsetCalculator())
engineer.add_feature(VelocitiesDominantFootCalculator())
engineer.add_feature(VelocitiesNonDominantFootCalculator())

calculated_features = engineer.engineer_features(augmented_samples)

# SPLIT PYTORCH DATASET
dataset = PassDataset(calculated_features)
torch.manual_seed(42)  # Set the seed
train_size = int(0.8 * len(dataset))
validation_size = len(dataset) - train_size
train_indices, val_indices = torch.utils.data.random_split(range(len(dataset)), [train_size, validation_size])

for i in train_indices:
    dataset.samples[i].inference.split = Split.TRAIN
for i in val_indices:
    dataset.samples[i].inference.split = Split.VALIDATION

# Create Subset datasets for training and validation
train_dataset = Subset(dataset, train_indices)
validation_dataset = Subset(dataset, val_indices)

positive_training = sum(1 for sample in dataset.samples if sample.inference.split == Split.TRAIN and sample.pass_info.is_a_pass)
negative_training = sum(1 for sample in dataset.samples if sample.inference.split == Split.TRAIN and not sample.pass_info.is_a_pass)
positive_validation = sum(1 for sample in dataset.samples if sample.inference.split == Split.VALIDATION and sample.pass_info.is_a_pass)
negative_validation = sum(1 for sample in dataset.samples if sample.inference.split == Split.VALIDATION and not sample.pass_info.is_a_pass)
print(f"Training set: Size:{train_size} Positive:{positive_training} Negative:{negative_training}")
print(f"Validation set: Size:{validation_size} Positive:{positive_validation} Negative:{negative_validation}")

# DataLoaders (if needed for further processing)
batch_size = 32
training_loader = DataLoader(train_dataset, batch_size=batch_size, shuffle=True)
validation_loader = DataLoader(validation_dataset, batch_size=batch_size, shuffle=False)

"""MODEL"""
input_size = engineer.input_size
hidden_size = 64
num_layers = 2
learning_rate = 0.001
num_epochs = 1000

model = PassDetectionModel(input_size, hidden_size, num_layers)
criterion = nn.BCELoss()
optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)

"""EARLY STOPPING"""
patience = 5
best_val_loss = float('inf')
epochs_no_improve = 0
early_stop = False

"""TRAINING"""
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
model.to(device)

epoch_index = 0
for epoch in range(num_epochs):
    epoch_index = epoch
    if early_stop:
        break

    model.train()
    total_loss = 0
    for inputs, labels in training_loader:
        inputs = inputs.to(device)
        labels = labels.to(device).view(-1, 1)
        optimizer.zero_grad()
        outputs = model(inputs)
        loss = criterion(outputs, labels)
        loss.backward()
        optimizer.step()
        total_loss += loss.item() * inputs.size(0)

    avg_loss = total_loss / len(training_loader.dataset)
    print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {avg_loss:.4f}")

    # Validation
    model.eval()
    total_val_loss = 0
    correct = 0
    with torch.no_grad():
        for inputs, labels in validation_loader:
            inputs = inputs.to(device)
            labels = labels.to(device).view(-1, 1)
            outputs = model(inputs)
            loss = criterion(outputs, labels)
            total_val_loss += loss.item() * inputs.size(0)
            predicted = (outputs >= 0.5).float()
            correct += (predicted == labels).sum().item()

    avg_val_loss = total_val_loss / len(validation_loader.dataset)
    accuracy = correct / len(validation_loader.dataset)
    print(f"Validation Loss: {avg_val_loss:.4f}, Accuracy: {accuracy:.4f}")

    # Check for improvement
    if avg_val_loss < best_val_loss:
        best_val_loss = avg_val_loss
        epochs_no_improve = 0
    else:
        epochs_no_improve += 1
        if epochs_no_improve >= patience:
            print(f'Early stopping at epoch {epoch + 1}')
            early_stop = True

"""SAVE MODEL"""
checkpoint = {
    'model_state_dict': model.state_dict(),
    'optimizer_state_dict': optimizer.state_dict(),
    'input_size': input_size,
    'hidden_size': hidden_size,
    'num_layers': num_layers,
    'epoch': epoch_index,
}
torch.save(checkpoint, 'pass_detection_model.pth')

"""EVALUATION"""
model.eval()
with torch.no_grad():
    for idx in range(len(dataset)):
        sample = dataset.samples[idx]
        input_tensor, label = dataset[idx]
        input_tensor = input_tensor.unsqueeze(0).to(device)
        output = model(input_tensor)
        probability = output.item()
        sample.inference.pass_probability = round(probability, 3)

print(f"Brier Score: {prediction_brier(dataset.samples):.4f}")
print(f"Accuracy: {prediction_accuracy(dataset.samples):.4f}")
precision, recall, f1_score = predict_precision_recall_f1(dataset.samples)
print(f"Precision: {precision:.4f}, Recall: {recall:.4f}, F1 Score: {f1_score:.4f}")

"""SAVE DATASET"""
# Save some to folder
with open('dataset.pkl', 'wb') as f:
    pickle.dump(dataset.samples, f)

with open('dataset.json', 'w') as f:
    json.dump([asdict(sample) for sample in dataset.samples], f, cls=CustomEncoder, indent=4)

"""PLOT SAMPLES"""
for idx, sample in enumerate(dataset.samples):
    if not save_plots:
        break

    filename = f"sample_{sample.trial_number}_{idx}_pass{sample.pass_info.is_a_pass}.png"
    fig = plot_sample_with_features(sample)
    plot_path = os.path.join(str(plot_dir), filename)
    fig.savefig(plot_path)
    plt.close(fig)

"""EXPORT ONNX"""
# Path for ONNX file
onnx_file_path = "pass_detection_model.onnx"
model.to('cpu')
model.eval()

# Grab only the first sample from the last batch of val_loader for export
example_input = inputs[0:1].cpu()  # First sample of last batch with batch size 1

# Evaluate the model with this single sample input (First sample from data)
with torch.no_grad():
    example_output = model(example_input)
    example_output_values = example_output.squeeze().tolist()

    print(f"Example input shape: {example_input.shape}")
    print(f"Example output shape: {example_output.shape}")

    print(f"Example input: {example_input}")
    print(f"Example output: {example_output_values}")

torch.onnx.export(
    model,
    example_input,
    onnx_file_path,
    export_params=True,
    opset_version=15,
    do_constant_folding=True,
    input_names=['input'],
    output_names=['output'],
)

onnx_model = onnx.load(onnx_file_path)
metadata_props = onnx_model.metadata_props.add()
metadata_props.key = "example_input"
metadata_props.value = str(example_input.tolist())  # Store example input as string

metadata_props = onnx_model.metadata_props.add()
metadata_props.key = "example_output"
metadata_props.value = str(example_output_values)  # Store example output as string
onnx.save(onnx_model, onnx_file_path)
