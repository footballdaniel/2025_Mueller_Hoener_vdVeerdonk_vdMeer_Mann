from dataclasses import dataclass
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain.repositories import Repository
from src.domain.samples import Sample
from src.services.feature_engineer import FeatureEngineer


@dataclass
class PassDataset(Dataset):

    def __init__(self, repo: Repository[Sample], number_samples: int, engineer: FeatureEngineer):
        self.repo = repo
        self.number_samples = number_samples
        self.engineer = engineer

    def __len__(self) -> int:
        return self.number_samples

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        sample = self.repo.get(idx)

        target = self.engineer.engineer(sample)
        self.repo.add(target)

        timeseries_length = len(sample.inference.targets[0].values)
        features_count = len(sample.inference.targets)

        flattened_values = []
        for feature in sample.inference.targets:
            flattened_values.extend(feature.values)

        input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
        input_tensor = input_tensor.view(timeseries_length, features_count)
        label = torch.tensor(sample.inference.outcome_label, dtype=torch.float32)

        return input_tensor, label
