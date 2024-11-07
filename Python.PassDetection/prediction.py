import pickle

import torch
from torch import device

from src.nn.lstm_model import PassDetectionModel
from src.nn.tensor_conversion import feature_to_input_tensor

# Load the checkpoint
checkpoint = torch.load('pass_detection_model.pth')

# Extract model parameters
input_size = checkpoint['input_size']
hidden_size = checkpoint['hidden_size']
num_layers = checkpoint['num_layers']

# Reconstruct the model
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
model = PassDetectionModel(input_size, hidden_size, num_layers)
model.load_state_dict(checkpoint['model_state_dict'])
model.to(device)
model.eval()

# Load data samples
with open('10_sample_passes.pkl', 'rb') as f:
    passes = pickle.load(f)
with open('10_sample_non_passes.pkl', 'rb') as f:
    non_passes = pickle.load(f)


# Function to predict and print results for a list of features
def predict_passes(features, label):
    predictions = []
    with torch.no_grad():
        for feature in features:
            input_tensor = feature_to_input_tensor(feature).unsqueeze(0).to(device)
            output = model(input_tensor)
            # Apply sigmoid to convert logits to probabilities
            probabilities = torch.sigmoid(output).squeeze()
            prediction = (probabilities >= 0.5).float().item()
            predictions.append(prediction)
            print(f'Actual label: {label}, Predicted label: {prediction}, Probability: {probabilities.item():.4f}')
    return predictions


# Predict for passes (label=1) and non-passes (label=0)
print("Predictions for Passes:")
predicted_passes = predict_passes(passes, label=1)

print("\nPredictions for Non-Passes:")
predicted_non_passes = predict_passes(non_passes, label=0)
