from dataclasses import dataclass
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain.samples import Sample


@dataclass
class PassDataset(Dataset):
    samples: List[Sample]

    def __len__(self) -> int:
        return len(self.samples)

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        sample = self.samples[idx]

        timeseries_length = len(sample.inference.targets[0].values)
        features_count = len(sample.inference.targets)

        flattened_values = []
        for feature in sample.inference.targets:
            flattened_values.extend(feature.values)

        input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
        input_tensor = input_tensor.view(timeseries_length, features_count)

        label = torch.tensor(sample.inference.outcome_label, dtype=torch.float32)

        return input_tensor, label
