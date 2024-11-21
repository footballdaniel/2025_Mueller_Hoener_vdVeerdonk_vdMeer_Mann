from __future__ import annotations

import abc
import math
from dataclasses import dataclass, field
from enum import Enum
from typing import List, Optional

import torch


class Foot(Enum):
    RIGHT = 0
    LEFT = 1

    def mirror(self):
        return Foot.LEFT if self == Foot.RIGHT else Foot.RIGHT


@dataclass
class Position:
    x: float
    y: float
    z: float

    def mirror_x(self):
        return Position(x=-self.x, y=self.y, z=self.z)

    def rotate_around_y(self, angle_degrees: float):
        """Rotate the position around the y-axis by the given angle in degrees."""
        angle_radians = math.radians(angle_degrees)
        cos_theta = math.cos(angle_radians)
        sin_theta = math.sin(angle_radians)
        x_new = self.x * cos_theta + self.z * sin_theta
        z_new = -self.x * sin_theta + self.z * cos_theta
        return Position(x=x_new, y=self.y, z=z_new)


@dataclass
class PassEvent:
    frame_number: int
    foot: Foot


@dataclass
class Trial:
    frame_rate_hz: int
    number_of_frames: int
    timestamps: List[float]
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Position]
    user_non_dominant_foot_positions: List[Position]
    pass_events: List[PassEvent] = field(default_factory=list)


@dataclass
class Sample:
    frame_rate_hz: int
    number_of_frames: int
    timestamps: List[float]
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Position]
    user_non_dominant_foot_positions: List[Position]
    is_a_pass: bool = False
    pass_id: Optional[int] = None
    pass_timestamp: Optional[float] = None
    pass_probability: Optional[float] = None

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
            is_a_pass=sample.is_a_pass,
            pass_id=sample.pass_id,
            pass_timestamp=sample.pass_timestamp,
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
            is_a_pass=sample.is_a_pass,
            pass_id=sample.pass_id,
            pass_timestamp=sample.pass_timestamp,
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
            is_a_pass=sample.is_a_pass,
            pass_id=sample.pass_id,
            pass_timestamp=sample.pass_timestamp,
        )


@dataclass
class AugmentedLabeledSample(Sample):
    rotation_angle: Optional[float] = None
    swapped_feet: bool = False


@dataclass
class Feature:
    name: str
    values: List[float]

    def to_tensor(self) -> torch.Tensor:
        return torch.tensor(self.values, dtype=torch.float32)


class Split(Enum):
    TRAIN = 0
    VALIDATION = 1
    TEST = 2


@dataclass
class SampleWithFeatures(AugmentedLabeledSample):
    features: List[Feature] = field(default_factory=list)
    output: int = 0
    split: Split = Split.TRAIN


class FeatureCalculator(abc.ABC):
    @property
    @abc.abstractmethod
    def size(self) -> int:
        """Return the size of the feature this calculator produces."""
        ...

    @abc.abstractmethod
    def calculate(self, sample: Sample) -> List[Feature]:
        """Calculate the feature for a given trial."""
        ...
