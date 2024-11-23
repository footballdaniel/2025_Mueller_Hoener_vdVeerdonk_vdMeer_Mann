from typing import List

from src.domain.common import Vector3
from src.domain.inferences import BaseFeature, Target
from src.domain.recordings import InputData


class ZeroedPositionDominantFoot(BaseFeature):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, input_data: InputData) -> List[Target]:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        zeroed_positions = [
            Vector3(p.x - origin.x, p.y - origin.y, p.z - origin.z) for p in dominant_positions
        ]
        x_values = [pos.x for pos in zeroed_positions]
        y_values = [pos.y for pos in zeroed_positions]
        z_values = [pos.z for pos in zeroed_positions]

        return [
            Target(name="zeroed_position_dominant_foot_x", values=x_values),
            Target(name="zeroed_position_dominant_foot_y", values=y_values),
            Target(name="zeroed_position_dominant_foot_z", values=z_values),
        ]
