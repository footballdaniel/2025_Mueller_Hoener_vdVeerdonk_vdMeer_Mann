from typing import List

from src.domain.common import Vector3
from src.domain.inferences import FeatureCalculator, Feature
from src.domain.recordings import InputData


class FootOffsetCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, input_data: InputData) -> List[Feature]:
        dominant_positions = input_data.user_dominant_foot_positions
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        offsets = [
            Vector3(
                non_dominant_positions[i].x - dominant_positions[i].x,
                non_dominant_positions[i].y - dominant_positions[i].y,
                non_dominant_positions[i].z - dominant_positions[i].z,
            )
            for i in range(len(dominant_positions))
        ]
        x_values = [pos.x for pos in offsets]
        y_values = [pos.y for pos in offsets]
        z_values = [pos.z for pos in offsets]

        return [
            Feature(name="offset_dominant_foot_to_non_dominant_foot_x", values=x_values),
            Feature(name="offset_dominant_foot_to_non_dominant_foot_y", values=y_values),
            Feature(name="offset_dominant_foot_to_non_dominant_foot_z", values=z_values),
        ]
