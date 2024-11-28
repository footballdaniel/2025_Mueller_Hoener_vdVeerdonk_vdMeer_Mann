from __future__ import annotations

import abc
from dataclasses import dataclass, field
from enum import Enum
from typing import List, Tuple

from src.domain.recordings import InputData
from src.features.feature_registry import FeatureRegistry


@dataclass(frozen=True)
class Input:
    name: str
    values: List[float]


class Feature(abc.ABC, metaclass=FeatureRegistry):

    @abc.abstractmethod
    def calculate(self, input_data: InputData) -> Input:
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
class ComputedFeatures:
    flattened_values: List[float]
    dimensions: Tuple[int, int]
    features: List[Input]


@dataclass(frozen=True)
class NoComputedFeatures(ComputedFeatures):

    def __init__(self):
        super().__init__(flattened_values=[], dimensions=(0, 0), features=[])


@dataclass(frozen=True)
class Inference:
    label: bool = False
    prediction: float = 0.0
    split: Split = Split.UNASSIGNED
    computed_features: ComputedFeatures = field(default_factory=NoComputedFeatures)


@dataclass(frozen=True)
class NoInference(Inference):
    pass
