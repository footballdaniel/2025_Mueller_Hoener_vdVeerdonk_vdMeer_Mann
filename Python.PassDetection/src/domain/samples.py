from __future__ import annotations

from dataclasses import dataclass, field
from enum import Enum
from pathlib import Path
from typing import Optional, List

from src.domain.common import Vector3


class IngestableRecording:
    def __init__(self, stem: str):
        self.event_file: Optional[Path] = None
        self.timeseries_file: Optional[Path] = None
        self.stem = stem

    def add_event_file(self, file: Path) -> None:
        self.event_file = file

    def add_timeseries_file(self, file: Path) -> None:
        self.timeseries_file = file

    def both_files_present(self) -> bool:
        return self.event_file is not None and self.timeseries_file is not None

    def __eq__(self, other: IngestableRecording) -> bool:
        return self.stem == other.stem

    def __hash__(self):
        return hash(self.stem)


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


@dataclass
class Recording:
    frame_rate_hz: int
    number_of_frames: int
    trial_number: int
    duration: float
    user_dominant_foot_positions: List[Vector3]
    user_non_dominant_foot_positions: List[Vector3]
    timestamps: List[float]
    events: List[Event] = field(default_factory=list)


@dataclass
class Sample:
    id: int
    recording: Recording

    def contains_a_pass(self) -> int:
        return int(any([event.is_pass for event in self.recording.events]))
