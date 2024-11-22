from __future__ import annotations

import abc
from dataclasses import dataclass, field
from enum import Enum
from typing import List

import torch

from src.domain.recordings import InputData


class FeatureCalculator(abc.ABC):
    @property
    @abc.abstractmethod
    def size(self) -> int:
        """Return the size of the feature this calculator produces."""
        ...

    @abc.abstractmethod
    def calculate(self, input_data: InputData) -> List[Feature]:
        """Calculate the feature for a given trial."""
        ...


@dataclass
class Feature:
    name: str
    values: List[float]

    def to_tensor(self) -> torch.Tensor:
        return torch.tensor(self.values, dtype=torch.float32)


class Split(Enum):
    UNASSIGNED = 0
    TRAIN = 1
    VALIDATION = 2
    TEST = 3


@dataclass
class Inference:
    features: List[Feature] = field(default_factory=list)
    outcome_label: int = 0
    pass_probability: float = 0.0
    split: Split = Split.UNASSIGNED


@dataclass
class NoInference(Inference):
    pass
