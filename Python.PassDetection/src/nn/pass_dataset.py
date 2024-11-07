import torch
from torch.utils.data import Dataset

from src.nn.tensor_conversion import feature_to_input_tensor


class PassDataset(Dataset):
    def __init__(self, features):
        self.features = features

    def __len__(self):
        return len(self.features)

    def __getitem__(self, idx):
        feature = self.features[idx]
        input_tensor = feature_to_input_tensor(feature)  # Shape: (sequence_length, 12)
        label = torch.tensor(int(feature.is_a_pass), dtype=torch.float32)  # Convert bool to float tensor
        return input_tensor, label
