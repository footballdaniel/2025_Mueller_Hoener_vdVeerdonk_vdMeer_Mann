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
model = PassDetectionModel(input_size, hidden_size, num_layers)
model.load_state_dict(checkpoint['model_state_dict'])
model.to(device)
model.eval()


# Prepare an input sample
# Assume 'new_feature' is an instance of PositionAndVelocityFeature
input_tensor = feature_to_input_tensor(new_feature).unsqueeze(0)  # Add batch dimension
input_tensor = input_tensor.to(device)

# Make a prediction
with torch.no_grad():
    output = model(input_tensor)
    prediction = (output >= 0.5).float().item()
    print(f'Predicted label: {prediction}')