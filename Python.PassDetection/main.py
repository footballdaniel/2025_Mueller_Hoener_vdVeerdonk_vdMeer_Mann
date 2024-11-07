import glob
import os

import torch
import torch.nn as nn
from torch.utils.data import random_split, DataLoader

from src.io.raw_data_reader import read_trial_from_json, read_pass_events_from_csv
from src.nn.lstm_model import PassDetectionModel
from src.nn.pass_dataset import PassDataset
from src.plots import plot_feature
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

    print("Trial instance created successfully:", trial)
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
    augmented_passes.append(feature)  # Include the original feature

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


"""PYTORCH DATASET"""
# Initialize dataset and DataLoaders
dataset = PassDataset(combined_dataset)
train_size = int(0.8 * len(dataset))
val_size = len(dataset) - train_size
train_dataset, val_dataset = random_split(dataset, [train_size, val_size])
batch_size = 32
train_loader = DataLoader(train_dataset, batch_size=batch_size, shuffle=True)
val_loader = DataLoader(val_dataset, batch_size=batch_size, shuffle=False)

# Model parameters
input_size = 12
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

for epoch in range(num_epochs):
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
    print(f"Epoch [{epoch+1}/{num_epochs}], Loss: {avg_loss:.4f}")

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

        # Save the best model
        checkpoint = {
            'model_state_dict': model.state_dict(),
            'optimizer_state_dict': optimizer.state_dict(),
            'input_size': input_size,
            'hidden_size': hidden_size,
            'num_layers': num_layers,
            'epoch': epoch,
            'loss': avg_val_loss
        }
        torch.save(checkpoint, 'pass_detection_model.pth')
    else:
        epochs_no_improve += 1
        if epochs_no_improve >= patience:
            print(f'Early stopping at epoch {epoch+1}')
            early_stop = True
