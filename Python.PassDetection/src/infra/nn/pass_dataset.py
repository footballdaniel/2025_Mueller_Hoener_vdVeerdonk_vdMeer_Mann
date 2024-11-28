from dataclasses import dataclass
from typing import Tuple

import torch
from torch import Tensor
from torch.utils.data import Dataset

from src.domain.inferences import InputData
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

        input_data = InputData(
            sample.recording.user_dominant_foot_positions,
            sample.recording.user_non_dominant_foot_positions,
            sample.recording.timestamps
        )

        flattened_values = self.engineer.engineer(input_data)

        input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
        input_tensor = input_tensor.view(self.engineer.shape)
        label = torch.tensor(sample.contains_a_pass(), dtype=torch.float32)

        return input_tensor, label
