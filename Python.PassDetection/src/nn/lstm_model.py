import torch.nn as nn


class PassDetectionModel(nn.Module):
    def __init__(self, input_size, hidden_size, num_layers):
        super(PassDetectionModel, self).__init__()
        self.lstm = nn.LSTM(input_size=input_size, hidden_size=hidden_size, num_layers=num_layers, batch_first=True)
        self.fc = nn.Linear(hidden_size, 1)  # Output layer for binary classification
        self.sigmoid = nn.Sigmoid()  # To convert output to probability

    def forward(self, x):
        # x: (batch_size, sequence_length, input_size)
        lstm_out, _ = self.lstm(x)  # lstm_out: (batch_size, sequence_length, hidden_size)
        # Use the output of the last time step
        out = lstm_out[:, -1, :]  # (batch_size, hidden_size)
        out = self.fc(out)  # (batch_size, 1)
        out = self.sigmoid(out)  # (batch_size, 1)
        return out
