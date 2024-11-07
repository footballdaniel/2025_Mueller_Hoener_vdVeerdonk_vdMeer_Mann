from dataclasses import dataclass
from typing import List, Optional

from src.domain import Position


@dataclass
class PositionAndVelocityFeature:
    is_a_pass: bool
    zeroed_position_dominant_foot: List[Position]
    offset_dominant_foot_to_non_dominant_foot: List[Position]
    velocities_dominant_foot: List[Position]
    velocities_non_dominant_foot: List[Position]
    # metadata
    trial_number: int
    timestamps: List[float]
    start_time: float
    pass_event_timestamp: Optional[float] = None  # New field
    pass_event_index: Optional[int] = None
    rotation_angle: Optional[float] = None  # New field

    def rotate_around_y(self, angle_degrees: float):
        """Rotate all positions around the y-axis by the given angle."""
        rotated_feature = PositionAndVelocityFeature(
            is_a_pass=self.is_a_pass,
            zeroed_position_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
                                           self.zeroed_position_dominant_foot],
            offset_dominant_foot_to_non_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
                                                       self.offset_dominant_foot_to_non_dominant_foot],
            velocities_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in self.velocities_dominant_foot],
            velocities_non_dominant_foot=[pos.rotate_around_y(angle_degrees) for pos in
                                          self.velocities_non_dominant_foot],
            trial_number=self.trial_number,
            start_time=self.start_time,
            timestamps=self.timestamps.copy(),
            pass_event_timestamp=self.pass_event_timestamp,
            pass_event_index=self.pass_event_index,
            rotation_angle=angle_degrees
        )
        return rotated_feature

    def swap_feet(self):
        """Swap the dominant and non-dominant foot data."""
        swapped_feature = PositionAndVelocityFeature(
            is_a_pass=self.is_a_pass,
            zeroed_position_dominant_foot=self.zeroed_position_dominant_foot.copy(),  # Keep the positions the same
            offset_dominant_foot_to_non_dominant_foot=[Position(-pos.x, pos.y, pos.z) for pos in
                                                       self.offset_dominant_foot_to_non_dominant_foot],
            velocities_dominant_foot=self.velocities_non_dominant_foot.copy(),
            velocities_non_dominant_foot=self.velocities_dominant_foot.copy(),
            trial_number=self.trial_number,
            start_time=self.start_time,
            timestamps=self.timestamps.copy(),
            pass_event_timestamp=self.pass_event_timestamp,
            pass_event_index=self.pass_event_index
        )
        return swapped_feature
