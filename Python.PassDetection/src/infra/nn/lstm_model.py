import torch
import torch.nn as nn

from src.infra.nn.base_nn_model import BaseModel


class LSTMPassDetectionModel(BaseModel, nn.Module):
    def __init__(self, input_size, hidden_size: int = 64, num_layers: int = 2):
        super(LSTMPassDetectionModel, self).__init__()
        self.lstm = nn.LSTM(input_size=input_size, hidden_size=hidden_size, num_layers=num_layers, batch_first=True)
        self.fc = nn.Linear(hidden_size, 1)  # Output layer for binary classification
        self.h0 = nn.Parameter(torch.zeros(num_layers, 1, hidden_size))
        self.c0 = nn.Parameter(torch.zeros(num_layers, 1, hidden_size))

    def forward(self, x):
        lstm_out, _ = self.lstm(x)
        out = lstm_out[:, -1, :]
        out = self.fc(out)
        out = torch.sigmoid(out)
        return out
