from typing import List

from src.domain.inferences import Feature
from src.domain.recordings import InputData


class FootOffsetZ(Feature):
    def __init__(self):
        self._values = []

    def calculate(self, input_data: InputData) -> None:
        dominant_positions = input_data.user_dominant_foot_positions
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        self._values = [
            non_dominant_positions[i].z - dominant_positions[i].z
            for i in range(len(dominant_positions))
        ]

    @property
    def values(self) -> List[float]:
        return self._values
