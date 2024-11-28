from __future__ import annotations

from dataclasses import dataclass, field
from enum import Enum
from typing import List

from src.domain.common import Vector3


class Foot(Enum):
    UNASSIGNED = 0
    RIGHT = 1
    LEFT = 2

    def mirror(self):
        return Foot.LEFT if self == Foot.RIGHT else Foot.RIGHT


@dataclass(frozen=True)
class Event:
    is_pass: bool
    frame_number: int
    foot: Foot
    pass_id: int
    timestamp: float


@dataclass(frozen=True)
class InputData:
    """
    The input that is used to create the features, its the raw data that gets processed by the feature calculator.
    """
    user_dominant_foot_positions: List[Vector3] = field(default_factory=list)
    user_non_dominant_foot_positions: List[Vector3] = field(default_factory=list)
    timestamps: List[float] = field(default_factory=list)
    is_pass: bool = False


@dataclass(frozen=True)
class NoInputData(InputData):
    pass


@dataclass
class Recording:
    frame_rate_hz: int
    number_of_frames: int
    trial_number: int
    duration: float
    input_data: InputData = field(default_factory=InputData)
    events: List[Event] = field(default_factory=list)
