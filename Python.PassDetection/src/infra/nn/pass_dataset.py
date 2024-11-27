from dataclasses import dataclass
from typing import List, Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain.repositories import BaseRepository
from src.domain.samples import Sample
from src.services.feature_engineer import FeatureEngineer


@dataclass
class PassDataset(Dataset):

    def __init__(self, repo: BaseRepository[Sample], number_samples: int, engineer: FeatureEngineer):
        self.repo = repo
        self.number_samples = number_samples
        self.engineer = engineer

    def __len__(self) -> int:
        return self.number_samples

    def __getitem__(self, idx: int) -> Tuple[Tensor, Tensor]:
        sample = self.repo.get(idx)

        engineered_sample = self.engineer.engineer(sample)

        timeseries_length = len(engineered_sample.inference.targets[0].values)
        features_count = len(engineered_sample.inference.targets)

        flattened_values = []
        for feature in engineered_sample.inference.targets:
            flattened_values.extend(feature.values)

        input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
        input_tensor = input_tensor.view(timeseries_length, features_count)
        label = torch.tensor(engineered_sample.inference.outcome_label, dtype=torch.float32)

        return input_tensor, label
