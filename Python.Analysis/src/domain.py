import abc
from dataclasses import dataclass
from enum import Enum
from typing import List


class Condition(Enum):
    IN_SITU = 'InSitu'
    INTERACTION = 'Interaction'
    NO_INTERACTION = 'NoInteraction'
    NO_OPPONENT = 'NoOpponent'


@dataclass
class Position:
    x: float
    y: float
    z: float


class Footedness(Enum):
    RIGHT = 'Right'
    LEFT = 'Left'


class Side(Enum):
    DOMINANT = 'Dominant'
    NON_DOMINANT = 'NonDominant'
    UNKNOWN = 'Unknown'


@dataclass
class Foot:
    side: Side


@dataclass
class Action(abc.ABC):
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
    foot: Foot
    success: bool


class NoPass(Pass):

    def __init__(self, position: Position = Position(0.0, 0.0, 0.0), time: int = 0, foot: Foot = Foot(Side.UNKNOWN),
                 success: bool = False):
        self.position = position
        self.time = time
        self.foot = foot
        self.success = success


@dataclass
class Trial:
    timestamps: List[float]
    dominant_foot_positions: List[Position]
    non_dominant_foot_positions: List[Position]
    hip_positions: List[Position]
    opponent_hip_positions: List[Position]
    actions: List[Action]
    start: Action
    end: Action
    dominant_foot_side: Footedness
