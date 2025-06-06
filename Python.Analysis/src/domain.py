import abc
from dataclasses import dataclass
from enum import Enum
from typing import List

import numpy as np


class Condition(Enum):
    InSitu = "InSitu"
    Interaction = "Interaction"
    NoInteraction = "NoInteraction"
    NoOpponent = "NoOpponent"

    def __str__(self) -> str:
        if self == Condition.InSitu:
            return "In situ"
        elif self == Condition.Interaction:
            return "Virtual interactive"
        elif self == Condition.NoInteraction:
            return "Virtual non-interactive"
        elif self == Condition.NoOpponent:
            return "Virtual no-opponent"
        return self.__repr__()


@dataclass
class Position:
    x: float
    y: float
    z: float

    def distance_2d(self, other: 'Position') -> float:
        return np.sqrt((self.x - other.x) ** 2 + (self.z - other.z) ** 2)

    def magnitude(self):
        return (self.x ** 2 + self.y ** 2 + self.z ** 2) ** 0.5

    def interpolate(self, other: "Position", t: float) -> "Position":
        return Position(
            x=self.x + (other.x - self.x) * t,
            y=self.y + (other.y - self.y) * t,
            z=self.z + (other.z - self.z) * t
        )


class Footedness(Enum):
    Left = "Left"
    Right = "Right"


class Side(Enum):
    Dominant = 'Dominant'
    NonDominant = 'NonDominant'
    Unknown = 'Unknown'


@dataclass
class Foot:
    side: Side


@dataclass
class Action:
    position: Position
    time_index: int


class NoAction(Action):
    def __init__(self, position: Position = Position(0.0, 0.0, 0.0), time: int = 0):
        self.position = position
        self.time = time


@dataclass
class Touch(Action):
    foot: Foot


@dataclass
class Pass(Action):
    success: bool = True
    foot: Foot = None


class NoPass(Pass):
    def __init__(
            self,
            position: Position = Position(0.0, 0.0, 0.0),
            time: int = 0,
            foot: Foot = Foot(Side.Unknown),
            success: bool = False
    ):
        self.position = position
        self.time = time
        self.foot = foot
        self.success = success


class Persistence(abc.ABC):
    @abc.abstractmethod
    def add(self, trial):
        ...

    @abc.abstractmethod
    def save(self, trials: List["Trial"]):
        ...


@dataclass
class Trial:
    participant_id: int
    path: str
    condition: Condition
    trial_number: int
    timestamps: List[float]
    head_positions: List[Position]
    dominant_foot_positions: List[Position]
    non_dominant_foot_positions: List[Position]
    hip_positions: List[Position]
    opponent_hip_positions: List[Position]
    actions: List[Action]
    start: Action
    pass_event: Pass
    dominant_foot_side: Footedness
    has_missing_data: bool = False
    cluster_label: int = None


@dataclass
class TrialCollection:
    trials: List[Trial]

    def __iter__(self):
        return iter(self.trials)

    def __len__(self) -> int:
        return len(self.trials)
