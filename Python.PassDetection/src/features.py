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


class OffsetDominantFootToNonDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, trial: AugmentedLabeledSample) -> List[Feature]:
        dominant_positions = trial.user_dominant_foot_positions
        non_dominant_positions = trial.user_non_dominant_foot_positions
        offsets = [
            Position(
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


class VelocitiesDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, trial: AugmentedLabeledSample) -> List[Feature]:
        dominant_positions = trial.user_dominant_foot_positions
        timestamps = trial.timestamps
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


class VelocitiesNonDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # Each feature now has only one component (x, y, or z)

    def calculate(self, trial: AugmentedLabeledSample) -> List[Feature]:
        non_dominant_positions = trial.user_non_dominant_foot_positions
        timestamps = trial.timestamps
        velocities = []

        for i in range(1, len(non_dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6  # Avoid division by zero
            dx = (non_dominant_positions[i].x - non_dominant_positions[i - 1].x) / dt
            dy = (non_dominant_positions[i].y - non_dominant_positions[i - 1].y) / dt
            dz = (non_dominant_positions[i].z - non_dominant_positions[i - 1].z) / dt
            velocities.append(Position(dx, dy, dz))

        velocities.insert(0, Position(0, 0, 0))  # Insert a zero velocity at the start

        x_values = [vel.x for vel in velocities]
        y_values = [vel.y for vel in velocities]
        z_values = [vel.z for vel in velocities]

        return [
            Feature(name="velocities_non_dominant_foot_x", values=x_values),
            Feature(name="velocities_non_dominant_foot_y", values=y_values),
            Feature(name="velocities_non_dominant_foot_z", values=z_values),
        ]
