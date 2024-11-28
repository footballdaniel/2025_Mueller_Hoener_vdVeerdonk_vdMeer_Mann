from __future__ import annotations

import abc
from dataclasses import dataclass
from typing import List

from src.domain.common import Vector3
from src.features.feature_registry import FeatureRegistry


@dataclass(frozen=True)
class InputData:
    user_dominant_foot_positions: List[Vector3]
    user_non_dominant_foot_positions: List[Vector3]
    timestamps: List[float]


class Feature(abc.ABC, metaclass=FeatureRegistry):

    @abc.abstractmethod
    def calculate(self, input_data: InputData) -> List[float]:
        ...

    @property
    def name(self) -> str:
        return self.__class__.__name__
