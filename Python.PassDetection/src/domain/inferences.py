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


class Shape(tuple):
    def __new__(cls, *args):
        if not all(isinstance(arg, int) for arg in args):
            raise TypeError("All elements of a Shape must be integers.")
        return super().__new__(cls, args)

    def add_dimension(self, dimension: int) -> Shape:
        return Shape(*self, dimension)

    def add_batch_dimension(self, batch_size: int) -> Shape:
        return Shape(batch_size, *self)
