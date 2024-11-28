from typing import List, Iterator

from src.domain.samples import IngestableRecording, Sample
from src.services.label_creator import Sampler
from src.services.recording_parser import RecordingParser


class SampleGenerator:

    @staticmethod
    def generate(recordings: List[IngestableRecording]) -> Iterator[Sample]:
        current_id = 0
        for recording_info in recordings:
            recording = RecordingParser.parse_recording(recording_info.timeseries_file, recording_info.event_file)
            for sample in Sampler.generate(recording, start_id=current_id):
                yield sample
                current_id += 1
