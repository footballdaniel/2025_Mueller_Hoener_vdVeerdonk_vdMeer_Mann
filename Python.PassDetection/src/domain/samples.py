from __future__ import annotations

from dataclasses import dataclass, field
from typing import List

from src.domain.augmentations import Augmentation, NoAugmentation
from src.domain.common import Position
from src.domain.inferences import Inference, NoInference
from src.domain.passes import NoPass, Pass


@dataclass
class Sample:
    frame_rate_hz: int
    number_of_frames: int
    timestamps: List[float]
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Position]
    user_non_dominant_foot_positions: List[Position]
    pass_info: Pass = field(default_factory=NoPass)
    augmentation: Augmentation = field(default_factory=NoAugmentation)
    inference: Inference = field(default_factory=NoInference)

    @staticmethod
    def mirror(sample: Sample) -> Sample:
        """Return a mirrored version of the given sample."""
        return Sample(
            frame_rate_hz=sample.frame_rate_hz,
            number_of_frames=sample.number_of_frames,
            timestamps=sample.timestamps,
            trial_number=sample.trial_number,
            duration=sample.duration,
            user_dominant_foot_positions=[pos.mirror_x() for pos in sample.user_dominant_foot_positions],
            user_non_dominant_foot_positions=[pos.mirror_x() for pos in sample.user_non_dominant_foot_positions],
            pass_info=sample.pass_info,
            augmentation=sample.augmentation,
            inference=sample.inference,
        )

    @staticmethod
    def rotate_around_y(sample: Sample, angle_degrees: float) -> Sample:
        """Return a rotated version of the sample around the y-axis."""
        return Sample(
            frame_rate_hz=sample.frame_rate_hz,
            number_of_frames=sample.number_of_frames,
            timestamps=sample.timestamps,
            trial_number=sample.trial_number,
            duration=sample.duration,
            user_dominant_foot_positions=[pos.rotate_around_y(angle_degrees) for pos in
                                          sample.user_dominant_foot_positions],
            user_non_dominant_foot_positions=[pos.rotate_around_y(angle_degrees) for pos in
                                              sample.user_non_dominant_foot_positions],
            pass_info=sample.pass_info,
            augmentation=Augmentation(rotation_angle=angle_degrees, swapped_feet=sample.augmentation.swapped_feet),
            inference=sample.inference,
        )

    @staticmethod
    def swap_feet(sample: Sample) -> Sample:
        """Return a version of the sample with dominant and non-dominant foot positions swapped."""
        return Sample(
            frame_rate_hz=sample.frame_rate_hz,
            number_of_frames=sample.number_of_frames,
            timestamps=sample.timestamps,
            trial_number=sample.trial_number,
            duration=sample.duration,
            user_dominant_foot_positions=sample.user_non_dominant_foot_positions.copy(),
            user_non_dominant_foot_positions=sample.user_dominant_foot_positions.copy(),
            pass_info=sample.pass_info,
            augmentation=Augmentation(rotation_angle=sample.augmentation.rotation_angle, swapped_feet=True),
            inference=sample.inference,
        )
