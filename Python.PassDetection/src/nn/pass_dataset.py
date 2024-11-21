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

        # Convert each feature's values into a tensor and stack along the correct dimension
        input_tensor = torch.stack(
            [torch.tensor(feature.values, dtype=torch.float32) for feature in sample.inference.features],
            dim=1  # Stack along columns to form the shape (10, 12) or similar
        )

        label = torch.tensor(sample.inference.outcome_label, dtype=torch.float32)

        return input_tensor, label
