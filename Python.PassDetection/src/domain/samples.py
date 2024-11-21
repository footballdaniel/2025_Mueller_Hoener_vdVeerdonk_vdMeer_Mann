from __future__ import annotations
from dataclasses import dataclass, field, replace
from typing import List
from src.domain.augmentations import Augmentation, NoAugmentation
from src.domain.common import Position
from src.domain.inferences import Inference, NoInference
from src.domain.passes import Pass, NoPass


@dataclass(frozen=True)
class Sample:
    frame_rate_hz: int
    number_of_frames: int
    timestamps: List[float]
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Position] = field(default_factory=list)
    user_non_dominant_foot_positions: List[Position] = field(default_factory=list)
    pass_info: Pass = field(default_factory=NoPass)
    augmentation: Augmentation = field(default_factory=NoAugmentation)
    inference: Inference = field(default_factory=NoInference)

    @staticmethod
    def mirror(sample: Sample) -> Sample:
        return replace(
            sample,
            user_dominant_foot_positions=[pos.mirror_x() for pos in sample.user_dominant_foot_positions],
            user_non_dominant_foot_positions=[pos.mirror_x() for pos in sample.user_non_dominant_foot_positions],
        )

    @staticmethod
    def rotate_around_y(sample: Sample, angle_degrees: float) -> Sample:
        return replace(
            sample,
            user_dominant_foot_positions=[pos.rotate_around_y(angle_degrees) for pos in sample.user_dominant_foot_positions],
            user_non_dominant_foot_positions=[pos.rotate_around_y(angle_degrees) for pos in sample.user_non_dominant_foot_positions],
            augmentation=Augmentation(rotation_angle=angle_degrees, swapped_feet=sample.augmentation.swapped_feet),
        )

    @staticmethod
    def swap_feet(sample: Sample) -> Sample:
        return replace(
            sample,
            user_dominant_foot_positions=sample.user_non_dominant_foot_positions.copy(),
            user_non_dominant_foot_positions=sample.user_dominant_foot_positions.copy(),
            augmentation=Augmentation(rotation_angle=sample.augmentation.rotation_angle, swapped_feet=True),
        )
