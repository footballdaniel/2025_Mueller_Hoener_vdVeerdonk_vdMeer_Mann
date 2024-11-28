from __future__ import annotations

import abc
from dataclasses import dataclass, field
from enum import Enum
from typing import List, Tuple

from src.domain.recordings import InputData
from src.features.feature_registry import FeatureRegistry


class Feature(abc.ABC, metaclass=FeatureRegistry):

    @abc.abstractmethod
    def calculate(self, input_data: InputData) -> None:
        ...

    @property
    @abc.abstractmethod
    def values(self) -> List[float]:
        ...

    @property
    def name(self) -> str:
        return self.__class__.__name__


class Split(Enum):
    UNASSIGNED = 0
    TRAIN = 1
    VALIDATION = 2
    TEST = 3


@dataclass(frozen=True)
class EngineeredInputData:
    flattened_values: List[float]
    outcome: int
    dimensions: Tuple[int, int]
    features: List[Feature]


@dataclass(frozen=True)
class NoEngineeredInput(EngineeredInputData):

    def __init__(self):
        super().__init__(flattened_values=[], outcome=0, dimensions=(0, 0), features=[])


@dataclass(frozen=True)
class Inference:
    pass_probability: float = 0.0
    split: Split = Split.UNASSIGNED
    engineered_input: EngineeredInputData = field(default_factory=NoEngineeredInput)


@dataclass(frozen=True)
class NoInference(Inference):
    pass
