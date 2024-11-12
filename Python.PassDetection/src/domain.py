import abc
import math
from dataclasses import dataclass
from dataclasses import field
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
        y_new = self.y
        z_new = -self.x * sin_theta + self.z * cos_theta
        return Position(x=x_new, y=y_new, z=z_new)


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

    def mirror(self):
        mirrored = Trial(
            frame_rate_hz=self.frame_rate_hz,
            number_of_frames=self.number_of_frames,
            timestamps=self.timestamps,
            trial_number=self.trial_number,
            duration=self.duration,
            user_dominant_foot_positions=[
                pos.mirror_x() for pos in self.user_dominant_foot_positions
            ],
            user_non_dominant_foot_positions=[
                pos.mirror_x() for pos in self.user_non_dominant_foot_positions
            ],
            pass_events=[
                PassEvent(frame_number=event.frame_number, foot=event.foot.mirror())
                for event in self.pass_events
            ]
        )
        return mirrored

    def rotate_around_y(self, angle_degrees: float):
        """Rotate all positions in the trial around the y-axis by the given angle."""
        rotated = Trial(
            frame_rate_hz=self.frame_rate_hz,
            number_of_frames=self.number_of_frames,
            timestamps=self.timestamps,
            trial_number=self.trial_number,
            duration=self.duration,
            user_dominant_foot_positions=[
                pos.rotate_around_y(angle_degrees) for pos in self.user_dominant_foot_positions
            ],
            user_non_dominant_foot_positions=[
                pos.rotate_around_y(angle_degrees) for pos in self.user_non_dominant_foot_positions
            ],
            pass_events=self.pass_events  # Pass events are unchanged by rotation
        )
        return rotated

    def swap_feet(self):
        """Swap the dominant and non-dominant foot data."""
        swapped = Trial(
            frame_rate_hz=self.frame_rate_hz,
            number_of_frames=self.number_of_frames,
            timestamps=self.timestamps,
            trial_number=self.trial_number,
            duration=self.duration,
            user_dominant_foot_positions=self.user_non_dominant_foot_positions.copy(),
            user_non_dominant_foot_positions=self.user_dominant_foot_positions.copy(),
            pass_events=[
                PassEvent(frame_number=event.frame_number, foot=event.foot.mirror())
                for event in self.pass_events
            ]
        )
        return swapped


@dataclass
class LabeledTrial(Trial):
    is_a_pass: bool = False
    pass_id: Optional[int] = None


@dataclass
class AugmentedLabeledTrial(LabeledTrial):
    rotation_angle: Optional[float] = None


@dataclass
class Feature:
    name: str
    values: List[float]

    def to_tensor(self) -> torch.Tensor:
        return torch.tensor(self.values, dtype=torch.float32)


@dataclass
class CalculatedFeatures:
    features: List[Feature]
    outcome: int  # Binary outcome (e.g., 0 or 1 indicating pass or no pass)


class FeatureCalculator(abc.ABC):
    @property
    @abc.abstractmethod
    def size(self) -> int:
        """Return the size of the feature this calculator produces."""
        ...

    @abc.abstractmethod
    def calculate(self, trial: Trial) -> List[Feature]:
        """Calculate the feature for a given trial."""
        ...
