import csv
import json
from dataclasses import replace
from typing import List

from src.domain.common import Position
from src.domain.recordings import PassEvent, Foot, Recording


class RecordingParser:

    def __init__(self):
        self._recording = None

    @property
    def recording(self) -> Recording:
        return self._recording

    def read_recording_from_json(self, file_path: str) -> None:
        with open(file_path, "r") as f:
            data = json.load(f)
            recording = Recording(
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
        self._recording = recording

    def read_pass_events_from_csv(self, file_path: str) -> None:
        events = []
        if not self.recording:
            raise ValueError("Recording must be read before pass events can be read.")

        with open(file_path, newline='') as csvfile:
            reader = csv.reader(csvfile)
            for index, row in enumerate(reader):
                frame_number = int(row[0])
                foot = Foot.RIGHT if row[1] == 'R' else Foot.LEFT
                event = PassEvent(
                    frame_number=frame_number,
                    foot=foot,
                    is_a_pass=True,
                    pass_id=index,
                    timestamp=self.recording.timestamps[frame_number])
                events.append(event)

        self._recording = replace(self.recording, pass_events=events)
