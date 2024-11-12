from src.domain import FeatureCalculator, AugmentedLabeledTrial, Feature, Position


class ZeroedPositionDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # x, y, z for each position

    def calculate(self, trial: AugmentedLabeledTrial) -> Feature:
        dominant_positions = trial.user_dominant_foot_positions
        origin = dominant_positions[0]
        zeroed_positions = [
            Position(p.x - origin.x, p.y - origin.y, p.z - origin.z) for p in dominant_positions
        ]
        values = [coord for pos in zeroed_positions for coord in (pos.x, pos.y, pos.z)]
        return Feature(name="zeroed_position_dominant_foot", values=values)


class OffsetDominantFootToNonDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # x, y, z for each offset position

    def calculate(self, trial: AugmentedLabeledTrial) -> Feature:
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
        values = [coord for pos in offsets for coord in (pos.x, pos.y, pos.z)]
        return Feature(name="offset_dominant_foot_to_non_dominant_foot", values=values)


class VelocitiesDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # x, y, z for each velocity

    def calculate(self, trial: AugmentedLabeledTrial) -> Feature:
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

        # Insert a zero velocity at the start
        velocities.insert(0, Position(0, 0, 0))
        values = [coord for vel in velocities for coord in (vel.x, vel.y, vel.z)]
        return Feature(name="velocities_dominant_foot", values=values)


class VelocitiesNonDominantFootCalculator(FeatureCalculator):
    @property
    def size(self) -> int:
        return 3  # x, y, z for each velocity

    def calculate(self, trial: AugmentedLabeledTrial) -> Feature:
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

        # Insert a zero velocity at the start
        velocities.insert(0, Position(0, 0, 0))
        values = [coord for vel in velocities for coord in (vel.x, vel.y, vel.z)]
        return Feature(name="velocities_non_dominant_foot", values=values)

# from dataclasses import dataclass
# from typing import List, Optional
#
# from src.domain import Position
#
#
# @dataclass
# class PositionAndVelocityFeature:
#     is_a_pass: bool
#     zeroed_position_dominant_foot: List[Position]
#     offset_dominant_foot_to_non_dominant_foot: List[Position]
#     velocities_dominant_foot: List[Position]
#     velocities_non_dominant_foot: List[Position]
#     # metadata
#     trial_number: int
#     timestamps: List[float]
#     start_time: float
#     pass_event_timestamp: Optional[float] = None  # New field
#     pass_event_index: Optional[int] = None
#     rotation_angle: Optional[float] = None  # New field
#
#     def rotate_around_y(self, angle_degrees: float):
#         """Rotate all positions around the y-axis by the given angle."""
#         rotated_feature = PositionAndVelocityFeature(
#             is_a_pass=self.is_a_pass,
#             zeroed_position_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
#                                            self.zeroed_position_dominant_foot],
#             offset_dominant_foot_to_non_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
#                                                        self.offset_dominant_foot_to_non_dominant_foot],
#             velocities_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in self.velocities_dominant_foot],
#             velocities_non_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
#                                           self.velocities_non_dominant_foot],
#             trial_number=self.trial_number,
#             start_time=self.start_time,
#             timestamps=self.timestamps.copy(),
#             pass_event_timestamp=self.pass_event_timestamp,
#             pass_event_index=self.pass_event_index,
#             rotation_angle=angle_degrees
#         )
#         return rotated_feature
#
#     def swap_feet(self):
#         """Swap the dominant and non-dominant foot data."""
#         swapped_feature = PositionAndVelocityFeature(
#             is_a_pass=self.is_a_pass,
#             zeroed_position_dominant_foot=self.zeroed_position_dominant_foot.copy(),  # Keep the positions the same
#             offset_dominant_foot_to_non_dominant_foot=[Position(-pos.x, pos.y, pos.z) for pos in
#                                                        self.offset_dominant_foot_to_non_dominant_foot],
#             velocities_dominant_foot=self.velocities_non_dominant_foot.copy(),
#             velocities_non_dominant_foot=self.velocities_dominant_foot.copy(),
#             trial_number=self.trial_number,
#             start_time=self.start_time,
#             timestamps=self.timestamps.copy(),
#             pass_event_timestamp=self.pass_event_timestamp,
#             pass_event_index=self.pass_event_index
#         )
#         return swapped_feature
