import abc
from dataclasses import dataclass
from enum import Enum


# Data classes for positions, foot, and actions
@dataclass
class Position:
    x: float
    y: float
    z: float


class Side(Enum):
    LEFT = 'Left'
    RIGHT = 'Right'


@dataclass
class Foot:
    side: Side
    position: Position
    is_dominant: bool


# Abstract base class for actions
@dataclass
class Action(abc.ABC):
    position: Position
    time: float


@dataclass
class Touch(Action):
    foot: Foot


@dataclass
class Pass(Action):
    foot: Foot
    success: bool
