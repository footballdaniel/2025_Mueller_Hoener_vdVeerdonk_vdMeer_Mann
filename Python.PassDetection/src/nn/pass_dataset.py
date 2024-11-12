from dataclasses import dataclass, field
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain import AugmentedLabeledTrial, Feature, FeatureCalculator


@dataclass
class PassDataset(Dataset):
    samples: List[AugmentedLabeledTrial]
    feature_calculators: List[FeatureCalculator] = field(default_factory=list)

    def __len__(self) -> int:
        return len(self.samples)

    def add_feature(self, feature_calculator: FeatureCalculator):
        self.feature_calculators.append(feature_calculator)

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        trial = self.samples[idx]

        # engineer this trial
        features = []

        for feature_calculator in self.feature_calculators:
            feature = feature_calculator.calculate(trial)
            features.append(feature)

        input_tensor = torch.stack(
            [torch.tensor(feature.values, dtype=torch.float32) for feature in features]
        )

        label = torch.tensor(int(trial.is_a_pass), dtype=torch.float32)

        return input_tensor, label

    @property
    def input_size(self) -> int:
        total_size = 0
        for feature in self.feature_calculators:
            total_size += feature.size
        return total_size
