import csv
import json
from typing import List

from src.domain import PassEvent, Foot, Position, Trial


def read_pass_events_from_csv(file_path: str) -> List[PassEvent]:
    events = []
    with open(file_path, newline='') as csvfile:
        reader = csv.reader(csvfile)
        for row in reader:
            frame_number = int(row[0])
            foot_direction = Foot.RIGHT if row[1] == 'R' else Foot.LEFT
            event = PassEvent(frame_number=frame_number, foot=foot_direction)
            events.append(event)
    return events


def read_trial_from_json(file_path: str) -> Trial:
    with open(file_path, "r") as f:
        data = json.load(f)
        trial = Trial(
            frame_rate_hz=data['FrameRateHz'],
            number_of_frames=data['NumberOfFrames'],
            timestamps=data['Timestamps'],
            trial_number=data['TrialNumber'],
            duration=data['Duration'],
            user_dominant_foot_positions=[
                Position(**pos) for pos in data['UserDominantFootPositions']
            ],
            user_non_dominant_foot_positions=[
                Position(**pos) for pos in data['UserNonDominantFootPositions']
            ]
        )
    return trial
