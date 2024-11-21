from dataclasses import dataclass, field
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain import FeatureCalculator, SampleWithFeatures


@dataclass
class PassDataset(Dataset):
    samples: List[SampleWithFeatures]

    def __len__(self) -> int:
        return len(self.samples)

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        trial = self.samples[idx]

        # Convert each feature's values into a tensor and stack along the correct dimension
        input_tensor = torch.stack(
            [torch.tensor(feature.values, dtype=torch.float32) for feature in trial.features],
            dim=1  # Stack along columns to form the shape (10, 12) or similar
        )

        # Use the outcome as the label
        label = torch.tensor(trial.outcome, dtype=torch.float32)

        return input_tensor, label
