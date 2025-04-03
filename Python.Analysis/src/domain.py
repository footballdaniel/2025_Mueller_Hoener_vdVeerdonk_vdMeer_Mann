import abc
from dataclasses import dataclass
from enum import Enum
from typing import List
import glob

import numpy as np
from scipy.interpolate import interp1d
from scipy.signal import butter, filtfilt


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

    def __init__(
            self,
            position: Position = Position(0.0, 0.0, 0.0),
            time: int = 0,
            foot: Foot = Foot(Side.UNKNOWN),
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

    def distance_between_last_touch_and_pass(self):
        last_touch = next((action for action in reversed(self.actions) if isinstance(action, Touch)), None)
        distance = last_touch.position.distance_2d(self.pass_event.position)
        return distance

    def timing_between_last_touch_and_pass(self):
        last_touch = next((action for action in reversed(self.actions) if isinstance(action, Touch)), None)
        time_difference = self.timestamps[self.pass_event.time_index] - self.timestamps[last_touch.time_index]
        return time_difference

    def duration(self):
        first_action_time = self.timestamps[self.start.time_index]
        last_action_time = self.timestamps[self.pass_event.time_index]
        return last_action_time - first_action_time

    def number_of_touches(self):
        return len(self.actions)

    def average_interpersonal_distance(self):
        if self.condition == Condition.NO_OPPONENT:
            return 0

        total_distance = 0
        start_index = self.start.time_index
        end_index = self.pass_event.time_index

        for i in range(start_index, end_index):
            total_distance += self.hip_positions[i].distance_2d(self.opponent_hip_positions[i])
        return total_distance / (end_index - start_index)

    def interpersonal_distance_at_pass_time(self):
        if self.condition == Condition.NO_OPPONENT:
            return 0

        position_of_player = self.hip_positions[self.pass_event.time_index]
        position_of_opponent = self.opponent_hip_positions[self.pass_event.time_index]

        return position_of_player.distance_2d(position_of_opponent)

    def butterworth_filter(self, data, cutoff=10, fs=100, order=3):
        nyquist = 0.5 * fs
        normal_cutoff = cutoff / nyquist
        b, a = butter(order, normal_cutoff, btype='low', analog=False)
        return filtfilt(b, a, data)

    def upsample(self, data, timestamps, target_fs=100):
        target_times = np.arange(timestamps[0], timestamps[-1], 1 / target_fs)
        interp_func = interp1d(timestamps, data, kind="linear", fill_value="extrapolate")
        return target_times, interp_func(target_times)

    def number_lateral_changes_of_direction(self, cutoff=1, target_fs=100, order=3):
        raw_timestamps = np.array(self.timestamps)  # Original timestamps (100ms interval)
        z_positions = np.array([pos.z for pos in self.head_positions])  # Z positions of user's head

        # Filter timestamps and z positions to match the start and end of the trial
        z_positions = z_positions[self.start.time_index:self.pass_event.time_index]
        raw_timestamps = raw_timestamps[self.start.time_index:self.pass_event.time_index]

        upsampled_times, upsampled_z = self.upsample(z_positions, raw_timestamps, target_fs=target_fs)
        filtered_z = self.butterworth_filter(upsampled_z, cutoff=cutoff, fs=target_fs, order=order)

        # Compute sign changes in derivative
        derivative = np.diff(filtered_z)
        return np.sum(derivative[1:] * derivative[:-1] < 0)

    def time_between_last_change_of_direction_and_pass(self) -> float:
        # Get the Z positions of the head
        z_positions = np.array([pos.z for pos in self.head_positions])
        
        # Apply Butterworth filter to the Z positions
        filtered_z = self.butterworth_filter(z_positions)

        # Compute the derivative of the filtered Z positions
        derivative = np.diff(filtered_z)

        last_change_index = None

        # Iterate through the derivative to find the last significant change
        for i in range(len(derivative) - 1, 0, -1):
            if derivative[i] * derivative[i - 1] < 0:  # Check for sign change
                last_change_index = i + 1  # +1 because of the diff operation
                break
        
        if last_change_index is not None and self.pass_event.time_index > last_change_index:
            time_difference = self.timestamps[self.pass_event.time_index] - self.timestamps[last_change_index]
            return time_difference
        
        # If no valid change of direction is found, return NaN
        return self.duration() # Return NaN if no valid change of direction is found


@dataclass
class TrialCollection:
    trials: List[Trial]

    def __iter__(self):
        return iter(self.trials)

    def __len__(self) -> int:
        return len(self.trials)
        
