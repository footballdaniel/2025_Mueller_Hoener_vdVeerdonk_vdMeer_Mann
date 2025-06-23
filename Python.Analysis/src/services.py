from typing import List
import numpy as np
from .domain import Trial, Touch, Position, Condition, TrialCollection
from .preprocessing import Filter


class DistanceCalculator:
    @staticmethod
    def distance_between_last_touch_and_pass(trial: Trial) -> float:
        last_touch = next((action for action in reversed(trial.actions) if isinstance(action, Touch)), None)
        if last_touch is None:
            return 0.0
        return last_touch.position.distance_2d(trial.pass_event.position)

    @staticmethod
    def average_interpersonal_distance(trial: Trial) -> float:
        if trial.condition == Condition.NO_OPPONENT:
            return 0

        total_distance = 0
        start_index = trial.start.time_index
        end_index = trial.pass_event.time_index

        for i in range(start_index, end_index):
            total_distance += trial.hip_positions[i].distance_2d(trial.opponent_hip_positions[i])
        return total_distance / (end_index - start_index)

    @staticmethod
    def interpersonal_distance_at_pass_time(trial: Trial) -> float:
        if trial.condition == Condition.NO_OPPONENT:
            return 0

        position_of_player = trial.hip_positions[trial.pass_event.time_index]
        position_of_opponent = trial.opponent_hip_positions[trial.pass_event.time_index]
        return position_of_player.distance_2d(position_of_opponent)


class TimeCalculator:
    @staticmethod
    def timing_between_last_touch_and_pass(trial: Trial) -> float:
        last_touch = next((action for action in reversed(trial.actions) if isinstance(action, Touch)), None)
        time_difference = trial.timestamps[trial.pass_event.time_index] - trial.timestamps[last_touch.time_index]
        return time_difference

    @staticmethod
    def duration(trial: Trial) -> float:
        first_action_time = trial.timestamps[trial.start.time_index]
        last_action_time = trial.timestamps[trial.pass_event.time_index]
        return last_action_time - first_action_time

    @staticmethod
    def time_between_last_change_of_direction_and_pass(trial: Trial) -> float:
        z_positions = [pos.z for pos in trial.hip_positions]
        z_positions = z_positions[trial.start.time_index:trial.pass_event.time_index]
        raw_timestamps = trial.timestamps[trial.start.time_index:trial.pass_event.time_index]
        z_positions = Filter.low_pass_filter(z_positions, raw_timestamps, cutoff=1, target_fs=100, order=3)

        derivative = np.diff(z_positions)
        indices_of_change = np.where(derivative[1:] * derivative[:-1] < 0)[0]
        last_change_index = indices_of_change[-1] if indices_of_change.size > 0 else None

        if last_change_index is not None:
            last_change_time = raw_timestamps[last_change_index]
            pass_time = trial.timestamps[trial.pass_event.time_index]
            return pass_time - last_change_time

        return TimeCalculator.duration(trial)


class MovementCalculator:
    @staticmethod
    def number_of_touches(trial: Trial) -> int:
        return len(trial.actions)

    @staticmethod
    def number_lateral_changes_of_direction(trial: Trial) -> int:
        z_positions = [pos.z for pos in trial.hip_positions]
        z_positions = z_positions[trial.start.time_index:trial.pass_event.time_index]
        raw_timestamps = trial.timestamps[trial.start.time_index:trial.pass_event.time_index]
        z_positions = Filter.low_pass_filter(z_positions, raw_timestamps, cutoff=1, target_fs=100, order=3)

        derivative = np.diff(z_positions)
        number_changes_direction = np.sum(derivative[1:] * derivative[:-1] < 0)
        return number_changes_direction


class OutlierCalculator:

    @staticmethod
    def number_of_touches_greater_than(trials: TrialCollection, threshold: int) -> int:
        count = 0
        for trial in trials:
            if MovementCalculator.number_of_touches(trial) > threshold:
                count += 1
        return count

    @staticmethod
    def duration_greater_than(trials: TrialCollection, threshold: float) -> int:
        count = 0
        for trial in trials:
            if TimeCalculator.duration(trial) > threshold:
                count += 1
        return count