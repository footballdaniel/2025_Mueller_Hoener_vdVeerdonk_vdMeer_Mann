import csv
from typing import List

from .domain import Trial, Persistence


class CSVPersistence(Persistence):
    def __init__(self):
        self.dependent_variables: List[List] = []

    def save(self, trials: List[Trial], output_file: str = "output.csv"):
        with open(output_file, "w", newline="") as csvfile:
            writer = csv.writer(csvfile)
            writer.writerow(
                [
                    "Participant",
                    "TrialNumber",
                    "Condition",
                    "Success",
                    "NumberOfTouches",
                    "NumberLateralChangesOfDirection",
                    "Duration",
                    "TimingBetweenLastTouchAndPass",
                    "AverageInterpersonalDistance",
                    "AverageInterpersonalDistanceAtPassTime",
                ]
            )
            writer.writerows(self.dependent_variables)

    def add(self, trial: Trial):
        if trial.has_missing_data:
            return

        data = [
            trial.participant_id,
            trial.trial_number,
            trial.condition.value,
            trial.pass_event.success,
            trial.number_of_touches(),
            round(trial.number_lateral_changes_of_direction(), 2),
            round(trial.duration(), 2),
            round(trial.timing_between_last_touch_and_pass(), 2),
            round(trial.average_interpersonal_distance(), 2),
            round(trial.interpersonal_distance_at_pass_time(), 2),
        ]

        self.dependent_variables.append(data)
