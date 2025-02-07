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

    def distance_2d(self, other):
        return ((self.x - other.x) ** 2 + (self.y - other.y) ** 2) ** 0.5

    def magnitude(self):
        return (self.x ** 2 + self.y ** 2 + self.z ** 2) ** 0.5

    def interpolate(self, other: "Position", t: float) -> "Position":
        return Position(
            x=self.x + (other.x - self.x) * t,
            y=self.y + (other.y - self.y) * t,
            z=self.z + (other.z - self.z) * t
        )


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
    path: str
    condition: Condition
    trial_number: int
    timestamps: List[float]
    dominant_foot_positions: List[Position]
    non_dominant_foot_positions: List[Position]
    hip_positions: List[Position]
    opponent_hip_positions: List[Position]
    actions: List[Action]
    start: Action
    end: Action
    dominant_foot_side: Footedness
    has_missing_data: bool = False

    def distance_between_last_touch_and_pass(self):
        last_touch = next((action for action in reversed(self.actions) if isinstance(action, Touch)), None)
        pass_event = next((action for action in self.actions if isinstance(action, Pass)), None)

        distance = last_touch.position.distance_2d(pass_event.position)
        return distance

    def timing_between_last_touch_and_pass(self):
        last_touch = next((action for action in reversed(self.actions) if isinstance(action, Touch)), None)
        pass_event = next((action for action in self.actions if isinstance(action, Pass)), None)

        time_difference = self.timestamps[pass_event.time_index] - self.timestamps[last_touch.time_index]
        return time_difference

    def duration(self):
        first_action = self.actions[0]
        last_action = self.actions[-1]
        first_action_time = self.timestamps[first_action.time_index]
        last_action_time = self.timestamps[last_action.time_index]
        return last_action_time - first_action_time

    def number_of_touches(self):
        return len(self.actions)

    def average_interpersonal_distance(self):
        if self.condition == Condition.NO_OPPONENT:
            return 0

        total_distance = 0
        for i in range(len(self.hip_positions)):
            total_distance += self.hip_positions[i].distance_2d(self.opponent_hip_positions[i])

        return total_distance / len(self.hip_positions)

    def interpersonal_distance_at_pass_time(self):
        if self.condition == Condition.NO_OPPONENT:
            return 0

        pass_event = next((action for action in self.actions if isinstance(action, Pass)), None)

        position_of_player = self.hip_positions[pass_event.time_index]
        position_of_opponent = self.opponent_hip_positions[pass_event.time_index]

        return position_of_player.distance_2d(position_of_opponent)
