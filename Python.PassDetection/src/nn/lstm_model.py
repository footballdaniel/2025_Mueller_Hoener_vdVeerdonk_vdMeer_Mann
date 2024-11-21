import torch
import torch.nn as nn


class PassDetectionModel(nn.Module):
    def __init__(self, input_size, hidden_size, num_layers):
        super(PassDetectionModel, self).__init__()
        self.hidden_size = hidden_size
        self.num_layers = num_layers
        self.lstm = nn.LSTM(input_size=input_size, hidden_size=hidden_size, num_layers=num_layers, batch_first=True)
        self.fc = nn.Linear(hidden_size, 1)

    def forward(self, x):
        # Automatically initialize states to zeros
        batch_size = x.size(0)
        h0 = torch.zeros(self.num_layers, batch_size, self.hidden_size).to(x.device)
        c0 = torch.zeros(self.num_layers, batch_size, self.hidden_size).to(x.device)

        # Forward propagate LSTM
        lstm_out, _ = self.lstm(x, (h0, c0))
        out = lstm_out[:, -1, :]
        out = self.fc(out)
        return torch.sigmoid(out)
