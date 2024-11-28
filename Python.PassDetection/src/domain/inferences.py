from __future__ import annotations

import abc
from dataclasses import dataclass, field
from enum import Enum
from typing import List

from src.domain.recordings import InputData, NoInputData, EngineeredInputData, NoEngineeredInput
from src.features.feature_registry import FeatureRegistry


class BaseFeature(abc.ABC, metaclass=FeatureRegistry):
    @property
    @abc.abstractmethod
    def size(self) -> int:
        ...

    @abc.abstractmethod
    def calculate(self, input_data: InputData) -> List[Target]:
        ...

    @property
    def name(self) -> str:
        return self.__class__.__name__


@dataclass(frozen=True)
class Target:
    """Represents a feature that has been calculated. This is part of the input for the model"""
    name: str
    values: List[float]


class Split(Enum):
    UNASSIGNED = 0
    TRAIN = 1
    VALIDATION = 2
    TEST = 3


@dataclass(frozen=True)
class Inference:
    pass_probability: float = 0.0
    split: Split = Split.UNASSIGNED
    engineered_input: EngineeredInputData = field(default_factory=NoEngineeredInput)


@dataclass(frozen=True)
class NoInference(Inference):
    pass
