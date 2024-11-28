from typing import List

from src.domain.inferences import Feature
from src.domain.recordings import InputData


class ZeroedPositionDominantFootX(Feature):
    def __init__(self):
        self._values = []

    @property
    def size(self) -> int:
        return 1

    def calculate(self, input_data: InputData) -> None:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        self._values = [
            p.x - origin.x for p in dominant_positions
        ]

    @property
    def values(self) -> List[float]:
        return self._values


class ZeroedPositionDominantFootY(Feature):
    def __init__(self):
        self._values = []

    @property
    def size(self) -> int:
        return 1

    def calculate(self, input_data: InputData) -> None:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        self._values = [
            p.y - origin.y for p in dominant_positions
        ]

    @property
    def values(self) -> List[float]:
        return self._values


class ZeroedPositionDominantFootZ(Feature):
    def __init__(self):
        self._values = []

    @property
    def size(self) -> int:
        return 1

    def calculate(self, input_data: InputData) -> None:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        self._values = [
            p.z - origin.z for p in dominant_positions
        ]

    @property
    def values(self) -> List[float]:
        return self._values
