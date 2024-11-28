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

        engineered_input = self.engineer.engineer(sample.recording.input_data)

        input_tensor = torch.tensor(engineered_input.flattened_values, dtype=torch.float32)
        input_tensor = input_tensor.view(engineered_input.dimensions[0], engineered_input.dimensions[1])
        label = torch.tensor(sample.recording.contains_a_pass, dtype=torch.float32)

        return input_tensor, label
