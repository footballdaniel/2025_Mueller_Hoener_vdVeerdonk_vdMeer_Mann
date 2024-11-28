import csv
import json
from dataclasses import replace
from pathlib import Path
from typing import Iterator, TypeVar

from src.domain.common import Vector3
from src.domain.samples import Sample, Foot, Event, Recording
from src.services.label_creator import Sampler

T = TypeVar('T', bound='RecordingParser')


class RecordingParser:

    @classmethod
    def parse_recording(cls, timeseries_file: Path, event_file: Path) -> Recording:
        recording = cls.load_recording_from_json(timeseries_file)
        recording = cls.load_pass_events_from_csv(event_file, recording)
        return recording

    @classmethod
    def load_recording_from_json(cls, file_path: Path) -> Recording:
        with open(file_path, "r") as f:
            data = json.load(f)
            recording = Recording(
                frame_rate_hz=data['FrameRateHz'],
                number_of_frames=data['NumberOfFrames'],
                trial_number=data['TrialNumber'],
                duration=data['Duration'],
                user_dominant_foot_positions=[Vector3(**pos) for pos in data['UserDominantFootPositions']],
                user_non_dominant_foot_positions=[Vector3(**pos) for pos in data['UserNonDominantFootPositions']],
                timestamps=data['Timestamps'],
                events=[]  # Initialize events as an empty list
            )
            return recording

    @classmethod
    def load_pass_events_from_csv(cls, file_path: Path, recording: Recording) -> Recording:
        events = []
        with open(file_path, newline='') as csvfile:
            reader = csv.reader(csvfile)
            for index, row in enumerate(reader):
                frame_number = int(row[0])
                foot = Foot.RIGHT if 'R' in row[1] else Foot.LEFT
                is_pass = 'P' in row[1]
                event = Event(
                    frame_number=frame_number,
                    foot=foot,
                    is_pass=is_pass,
                    pass_id=index,
                    timestamp=recording.timestamps[frame_number]
                )
                events.append(event)
        recording = replace(recording, events=events)
        return recording

    @classmethod
    def get_sample_iterator(cls, recording: Recording, start_id: int = 0) -> Iterator[Sample]:
        return Sampler.generate(recording, start_id=start_id)
