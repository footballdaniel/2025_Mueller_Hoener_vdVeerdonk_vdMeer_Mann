from typing import List

from src.domain.inferences import BaseFeature, Target
from src.domain.recordings import InputData


class VelocityMagnitudeDominantFoot(BaseFeature):
    @property
    def size(self) -> int:
        return 1  # Single component: magnitude of velocity

    def calculate(self, input_data: InputData) -> List[Target]:
        dominant_positions = input_data.user_dominant_foot_positions
        timestamps = input_data.timestamps
        magnitudes = []

        for i in range(1, len(dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6  # Avoid division by zero
            dx = (dominant_positions[i].x - dominant_positions[i - 1].x) / dt
            dy = (dominant_positions[i].y - dominant_positions[i - 1].y) / dt
            dz = (dominant_positions[i].z - dominant_positions[i - 1].z) / dt
            magnitude = (dx ** 2 + dy ** 2 + dz ** 2) ** 0.5
            magnitudes.append(magnitude)

        magnitudes.insert(0, 0.0)  # Insert a zero magnitude at the start

        return [
            Target(name="velocity_magnitude_dominant_foot", values=magnitudes)
        ]
