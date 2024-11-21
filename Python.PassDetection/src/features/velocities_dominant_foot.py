from typing import List

from src.domain.common import Position
from src.domain.inferences import FeatureCalculator, Feature
from src.domain.recordings import Recording


class VelocitiesDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, recording: Recording) -> List[Feature]:
        dominant_positions = recording.user_dominant_foot_positions
        timestamps = recording.timestamps
        velocities = []

        for i in range(1, len(dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6  # Avoid division by zero
            dx = (dominant_positions[i].x - dominant_positions[i - 1].x) / dt
            dy = (dominant_positions[i].y - dominant_positions[i - 1].y) / dt
            dz = (dominant_positions[i].z - dominant_positions[i - 1].z) / dt
            velocities.append(Position(dx, dy, dz))

        velocities.insert(0, Position(0, 0, 0))  # Insert a zero velocity at the start

        x_values = [vel.x for vel in velocities]
        y_values = [vel.y for vel in velocities]
        z_values = [vel.z for vel in velocities]

        return [
            Feature(name="velocities_dominant_foot_x", values=x_values),
            Feature(name="velocities_dominant_foot_y", values=y_values),
            Feature(name="velocities_dominant_foot_z", values=z_values),
        ]
