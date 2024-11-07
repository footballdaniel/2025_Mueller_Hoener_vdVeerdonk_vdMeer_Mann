import glob
import os
import pickle

import torch
import torch.nn as nn
from torch import sigmoid
from torch.utils.data import random_split, DataLoader

from src.io.raw_data_reader import read_trial_from_json, read_pass_events_from_csv
from src.nn.brier import calculate_brier_score
from src.nn.f1 import calculate_classification_metrics
from src.nn.lstm_model import PassDetectionModel
from src.nn.pass_dataset import PassDataset
from src.training_data_sampler import DataSampler

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

"""SAMPLER"""
sampler = DataSampler(trials)
full_dataset = sampler.generate_dataset()
# for sample in full_dataset:
#     plot_feature(sample, 'plots', f'{sample.trial_number}_{sample.start_time}_{sample.is_a_pass}.png')


"""AUGMENTATION"""
rotation_angles = [angle for angle in range(5, 360, 10)]
augmented_passes = []
for feature in full_dataset:

    if feature.is_a_pass:
        for angle in rotation_angles:
            rotated_feature = feature.rotate_around_y(angle)
            augmented_passes.append(rotated_feature)

        swapped_feature = feature.swap_feet()
        augmented_passes.append(swapped_feature)

        for angle in rotation_angles:
            swapped_rotated_feature = swapped_feature.rotate_around_y(angle)
            augmented_passes.append(swapped_rotated_feature)

combined_dataset = full_dataset + augmented_passes

# Save some to folder
with open('10_sample_passes.pkl', 'wb') as f:
    pickle.dump(augmented_passes[:10], f)
# save 10 non-passes
non_passes = [sample for sample in combined_dataset if not sample.is_a_pass]
with open('10_sample_non_passes.pkl', 'wb') as f:
    pickle.dump(non_passes[:10], f)

"""PYTORCH DATASET"""
torch.manual_seed(42)  # Set the seed
dataset = PassDataset(combined_dataset)
train_size = int(0.8 * len(dataset))
val_size = len(dataset) - train_size
train_dataset, val_dataset = random_split(dataset, [train_size, val_size])
batch_size = 32
train_loader = DataLoader(train_dataset, batch_size=batch_size, shuffle=True)
val_loader = DataLoader(val_dataset, batch_size=batch_size, shuffle=False)

# Count positive and negative samples in combined_dataset
num_positive = sum(1 for feature in combined_dataset if feature.is_a_pass)
num_negative = sum(1 for feature in combined_dataset if not feature.is_a_pass)
print(f"Total samples: {len(combined_dataset)}")
print(f"Positive samples: {num_positive}")
print(f"Negative samples: {num_negative}")

# For the training and validation sets
# Since train_dataset and val_dataset are Subset objects, we can access the indices
train_indices = train_dataset.indices
val_indices = val_dataset.indices

# Count positive and negative samples in the training set
train_positive = sum(1 for idx in train_indices if dataset.features[idx].is_a_pass)
train_negative = len(train_indices) - train_positive
print(f"Training set - Positive: {train_positive}, Negative: {train_negative}")

# Count positive and negative samples in the validation set
val_positive = sum(1 for idx in val_indices if dataset.features[idx].is_a_pass)
val_negative = len(val_indices) - val_positive
print(f"Validation set - Positive: {val_positive}, Negative: {val_negative}")

# Model parameters
input_size = 12
hidden_size = 64
num_layers = 2
learning_rate = 0.001
num_epochs = 50  # Increase because early stopping will determine the actual number of epochs

# Initialize model, loss function, optimizer
model = PassDetectionModel(input_size, hidden_size, num_layers)
criterion = nn.BCEWithLogitsLoss()
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
    for inputs, labels in train_loader:
        inputs = inputs.to(device)
        labels = labels.to(device).view(-1, 1)
        optimizer.zero_grad()
        outputs = model(inputs)
        loss = criterion(outputs, labels)
        loss.backward()
        optimizer.step()
        total_loss += loss.item() * inputs.size(0)

    avg_loss = total_loss / len(train_loader.dataset)
    print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {avg_loss:.4f}")

    # Validation
    model.eval()
    total_val_loss = 0
    correct = 0
    with torch.no_grad():
        for inputs, labels in val_loader:
            inputs = inputs.to(device)
            labels = labels.to(device).view(-1, 1)
            outputs = model(inputs)
            loss = criterion(outputs, labels)
            total_val_loss += loss.item() * inputs.size(0)
            predicted = (outputs >= 0.5).float()
            correct += (predicted == labels).sum().item()

    avg_val_loss = total_val_loss / len(val_loader.dataset)
    accuracy = correct / len(val_loader.dataset)
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
    for inputs, labels in val_loader:
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

avg_val_loss = total_val_loss / len(val_loader.dataset)
accuracy = correct / len(val_loader.dataset)
print(f"Validation Loss: {avg_val_loss:.4f}, Accuracy: {accuracy:.4f}")

# Calculate Brier score
brier_score = calculate_brier_score(torch.tensor(all_labels), torch.tensor(all_probabilities))
print(f"Brier Score: {brier_score:.4f}")

# Calculate precision, recall, F1 score
precision, recall, f1 = calculate_classification_metrics(torch.tensor(all_labels), torch.tensor(all_predictions))
print(f"Precision: {precision:.4f}, Recall: {recall:.4f}, F1 Score: {f1:.4f}")

"""EVALUATE FIRST BATCH FOR SANITY"""
with torch.no_grad():
    for inputs, labels in val_loader:
        inputs = inputs.to(device)
        labels = labels.to(device).view(-1, 1)
        outputs = model(inputs)
        probabilities = sigmoid(outputs).squeeze()
        predicted = (probabilities >= 0.5).float()
        # Print some outputs and labels
        print(f"Probabilities: {probabilities[:5]}")
        print(f"Predicted labels: {predicted[:5]}")
        print(f"Actual labels: {labels.squeeze()[:5]}")
        # Break after first batch for brevity
        break

"""EXPORT ONNX"""
# Define the output file name
onnx_file_path = "pass_detection_model.onnx"
# Ensure the model is on the CPU
model.to('cpu')
# Export the model
sequence_length = 10  # same as training data
dummy_input = torch.randn(batch_size, sequence_length, input_size)
# Export the model
torch.onnx.export(
    model,  # Model being run
    dummy_input,  # Model input (or a tuple for multiple inputs)
    onnx_file_path,  # Where to save the model
    export_params=True,  # Store the trained parameter weights inside the model file
    opset_version=11,  # ONNX version to export to (11 is widely supported)
    do_constant_folding=True,  # Whether to execute constant folding for optimization
    input_names=['input'],  # Model's input names
    output_names=['output'],  # Model's output names
    dynamic_axes={
        'input': {0: 'batch_size', 1: 'sequence_length'},  # Variable batch size and sequence length
        'output': {0: 'batch_size'}  # Variable batch size
    }
)
