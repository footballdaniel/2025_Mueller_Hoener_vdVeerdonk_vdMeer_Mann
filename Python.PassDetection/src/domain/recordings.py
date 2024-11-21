from __future__ import annotations

from dataclasses import dataclass, field
from enum import Enum
from typing import List

from src.domain.common import Position


class Foot(Enum):
    UNASSIGNED = 0
    RIGHT = 1
    LEFT = 2

    def mirror(self):
        return Foot.LEFT if self == Foot.RIGHT else Foot.RIGHT


@dataclass(frozen=True)
class PassEvent:
    is_a_pass: bool
    frame_number: int
    foot: Foot
    pass_id: int
    timestamp: float


@dataclass
class Recording:
    frame_rate_hz: int
    number_of_frames: int
    timestamps: List[float]
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Position]
    user_non_dominant_foot_positions: List[Position]
    pass_events: List[PassEvent] = field(default_factory=list)
