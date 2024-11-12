from typing import List

from src.domain import FeatureCalculator, AugmentedLabeledSample, Feature, Position


class ZeroedPositionDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, trial: AugmentedLabeledSample) -> List[Feature]:
        dominant_positions = trial.user_dominant_foot_positions
        origin = dominant_positions[0]
        zeroed_positions = [
            Position(p.x - origin.x, p.y - origin.y, p.z - origin.z) for p in dominant_positions
        ]
        x_values = [pos.x for pos in zeroed_positions]
        y_values = [pos.y for pos in zeroed_positions]
        z_values = [pos.z for pos in zeroed_positions]

        return [
            Feature(name="zeroed_position_dominant_foot_x", values=x_values),
            Feature(name="zeroed_position_dominant_foot_y", values=y_values),
            Feature(name="zeroed_position_dominant_foot_z", values=z_values),
        ]
