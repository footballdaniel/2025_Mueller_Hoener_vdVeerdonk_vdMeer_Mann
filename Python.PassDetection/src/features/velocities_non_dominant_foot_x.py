from typing import List

from src.domain.inferences import Feature, InputData


class VelocityNonDominantFootX(Feature):

    def calculate(self, input_data: InputData) -> List[float]:
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        timestamps = input_data.timestamps
        values = []

        for i in range(1, len(non_dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6
            dx = (non_dominant_positions[i].x - non_dominant_positions[i - 1].x) / dt
            values.append(dx)

        values.insert(0, 0.0)  # Insert a zero velocity at the start

        return values

