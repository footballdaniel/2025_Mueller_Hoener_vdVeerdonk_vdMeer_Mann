from typing import List

from src.domain.inferences import Feature
from src.domain.recordings import InputData


class VelocityNonDominantFootY(Feature):
    def __init__(self):
        self._values = []

    def calculate(self, input_data: InputData) -> None:
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        timestamps = input_data.timestamps
        self._values = []

        for i in range(1, len(non_dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6
            dy = (non_dominant_positions[i].y - non_dominant_positions[i - 1].y) / dt
            self._values.append(dy)

        self._values.insert(0, 0.0)  # Insert a zero velocity at the start

    @property
    def values(self) -> List[float]:
        return self._values
