from dataclasses import dataclass, field
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain import FeatureCalculator, CalculatedFeatures


@dataclass
class PassDataset(Dataset):
    calculated_features: List[CalculatedFeatures]
    feature_calculators: List[FeatureCalculator] = field(default_factory=list)

    def __len__(self) -> int:
        return len(self.calculated_features)

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        trial = self.calculated_features[idx]

        # Convert each feature's values into a tensor and stack along the correct dimension
        input_tensor = torch.stack(
            [torch.tensor(feature.values, dtype=torch.float32) for feature in trial.features],
            dim=1  # Stack along columns to form the shape (10, 12) or similar
        )

        # Use the outcome as the label
        label = torch.tensor(trial.outcome, dtype=torch.float32)

        return input_tensor, label

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)