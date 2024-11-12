import glob
import os

import onnx
import torch
import torch.nn as nn
from torch.utils.data import random_split, DataLoader

from src.feature_engineer import FeatureEngineer
from src.features import VelocitiesNonDominantFootCalculator, VelocitiesDominantFootCalculator
from src.io.raw_data_reader import read_trial_from_json, read_pass_events_from_csv
from src.nn.brier import calculate_brier_score
from src.nn.f1 import calculate_classification_metrics
from src.nn.lstm_model import PassDetectionModel
from src.nn.pass_dataset import PassDataset
from src.trial_augmenter import TrialAugmenter
from src.trial_labeler import TrialLabeler

# Use glob to find all CSV files
pattern = "data/**/*.csv"

trials = []
for filename in glob.iglob(pattern, recursive=True):
    print(f"Processing file: {filename}")
    json_filename = filename.replace(".csv", ".json")
    if not os.path.isfile(json_filename):
        print("No JSON file found for", filename)
        continue

    trial = read_trial_from_json(json_filename)
    pass_events = read_pass_events_from_csv(filename)
    trial.pass_events.extend(pass_events)  # Append the events to the Trial
    trials.append(trial)

"""SAMPLING AND AUGMENTATION"""
labeled_trials = TrialLabeler.generate_dataset(trials)
augmented_trials = TrialAugmenter.augment(labeled_trials)

"""FEATURE ENGINEERING"""
engineer = FeatureEngineer()
velocity_non_dominant_foot = VelocitiesNonDominantFootCalculator()
velocity_dominant_foot = VelocitiesDominantFootCalculator()
offset_dominant_foot_to_non_dominant_foot = VelocitiesDominantFootCalculator()
zeroed_dominant_foot_positions = VelocitiesDominantFootCalculator()

engineer.add_feature(velocity_non_dominant_foot)
engineer.add_feature(velocity_dominant_foot)
engineer.add_feature(offset_dominant_foot_to_non_dominant_foot)
engineer.add_feature(zeroed_dominant_foot_positions)

calculated_features = engineer.engineer_features(augmented_trials)


# # Save some to folder
# with open('10_sample_passes.pkl', 'wb') as f:
#     pickle.dump(augmented_trials[:10], f)
# # save 10 non-passes
# non_passes = [sample for sample in labeled_trials if not sample.is_a_pass]
# with open('10_sample_non_passes.pkl', 'wb') as f:
#     pickle.dump(non_passes[:10], f)
#
# for sample in labeled_trials:
#     plot_labeled_trial(sample, 'plots', f'{sample.trial_number}_{round(sample.timestamps[0],2)}_{sample.is_a_pass}.png')

"""SPLIT PYTORCH DATASET"""
dataset = PassDataset(calculated_features)
torch.manual_seed(42)  # Set the seed
train_size = int(0.8 * len(dataset))
val_size = len(dataset) - train_size
train_dataset, val_dataset = random_split(dataset, [train_size, val_size])
batch_size = 32
training_loader = DataLoader(train_dataset, batch_size=batch_size, shuffle=True)
validation_loader = DataLoader(val_dataset, batch_size=batch_size, shuffle=False)

# Count positive and negative samples in combined_dataset
num_positive = sum(1 for feature in augmented_trials if feature.is_a_pass)
num_negative = sum(1 for feature in augmented_trials if not feature.is_a_pass)
print(f"Total samples: {len(augmented_trials)}")
print(f"Positive samples: {num_positive}")
print(f"Negative samples: {num_negative}")

# Model parameters
input_size = dataset.input_size
hidden_size = 64
num_layers = 2
learning_rate = 0.001
num_epochs = 50  # Increase because early stopping will determine the actual number of epochs

# Initialize model, loss function, optimizer
model = PassDetectionModel(input_size, hidden_size, num_layers)
criterion = nn.BCELoss()
optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)

# Early stopping parameters
patience = 5
best_val_loss = float('inf')
epochs_no_improve = 0
early_stop = False

# Training loop
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
model.to(device)

epoch_index = 0
for epoch in range(num_epochs):
    epoch_index = epoch
    if early_stop:
        print("Early stopping")
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

# Save the best model
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
# Validation
model.eval()
total_val_loss = 0
correct = 0
all_labels = []
all_predictions = []
all_probabilities = []

with torch.no_grad():
    for inputs, labels in validation_loader:
        inputs = inputs.to(device)
        labels = labels.to(device).view(-1, 1)
        outputs = model(inputs)
        loss = criterion(outputs, labels)
        total_val_loss += loss.item() * inputs.size(0)
        probabilities = outputs.squeeze()
        predicted = (probabilities >= 0.5).float()
        correct += (predicted == labels.squeeze()).sum().item()

        # Collect labels and predictions for metrics
        all_labels.extend(labels.squeeze().cpu())
        all_predictions.extend(predicted.cpu())
        all_probabilities.extend(probabilities.cpu())

avg_val_loss = total_val_loss / len(validation_loader.dataset)
accuracy = correct / len(validation_loader.dataset)
print(f"Validation Loss: {avg_val_loss:.4f}, Accuracy: {accuracy:.4f}")

# Calculate Brier score
brier_score = calculate_brier_score(torch.tensor(all_labels), torch.tensor(all_probabilities))
print(f"Brier Score: {brier_score:.4f}")

# Calculate precision, recall, F1 score
precision, recall, f1 = calculate_classification_metrics(torch.tensor(all_labels), torch.tensor(all_predictions))
print(f"Precision: {precision:.4f}, Recall: {recall:.4f}, F1 Score: {f1:.4f}")

"""RUN MODEL ON FIRST BATCH FOR SANITY CHECK"""
with torch.no_grad():
    for inputs, labels in validation_loader:
        inputs = inputs.to(device)
        labels = labels.to(device)
        outputs = model(inputs)
        probabilities = outputs.squeeze()
        predicted = (probabilities >= 0.5).float()
        print(f"Probabilities: {probabilities[:5]}")
        print(f"Predicted labels: {predicted[:5]}")
        print(f"Actual labels: {labels.squeeze()[:5]}")
        # Only print first batch, continue to export
        break

"""EXPORT ONNX"""
# Path for ONNX file
onnx_file_path = "pass_detection_model.onnx"
model.to('cpu')
model.eval()

# Grab only the first sample from the first batch of val_loader for export
example_input = inputs[0:1].cpu()  # First sample with batch size 1

# Evaluate the model with this single sample input (First sample from data)
with torch.no_grad():
    example_output = model(example_input)
    example_output_values = example_output.squeeze().tolist()

    print(f"Example input shape: {example_input.shape}")
    print(f"Example output shape: {example_output.shape}")

    print(f"Example input: {example_input}")
    print(f"Example output: {example_output_values}")

# Export the model using the first sample as the dummy input
torch.onnx.export(
    model,
    example_input,
    onnx_file_path,
    export_params=True,
    opset_version=11,
    do_constant_folding=True,
    input_names=['input'],
    output_names=['output'],
    dynamic_axes={
        'input': {0: 'batch_size', 1: 'sequence_length'},
        'output': {0: 'batch_size'}
    }
)

# Load the exported ONNX model and add metadata
onnx_model = onnx.load(onnx_file_path)

# Adding metadata for example input and output
metadata_props = onnx_model.metadata_props.add()
metadata_props.key = "example_input"
metadata_props.value = str(example_input.tolist())  # Store example input as string

metadata_props = onnx_model.metadata_props.add()
metadata_props.key = "example_output"
metadata_props.value = str(example_output_values)  # Store example output as string

# Save the modified model with added metadata
onnx.save(onnx_model, onnx_file_path)
