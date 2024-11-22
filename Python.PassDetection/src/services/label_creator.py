from dataclasses import replace
from typing import List, Optional

from src.domain.recordings import Recording, PassEvent, Foot
from src.domain.samples import Sample


class LabelCreator:

    @staticmethod
    def generate(recordings: List[Recording], sequence_length: int = 10) -> List[Sample]:
        """Generate labeled samples for a list of recordings."""
        return [
            sample
            for recording in recordings
            for sample in LabelCreator._create_sequences(recording, sequence_length)
        ]

    @staticmethod
    def _create_sequences(recording: Recording, sequence_length: int) -> List[Sample]:
        """Create labeled sequences from a single recording."""
        samples = []
        total_samples = len(recording.input_data.user_dominant_foot_positions)

        for start_idx in range(total_samples - sequence_length + 1):
            pass_event = LabelCreator._get_pass_event_in_sequence(recording, start_idx, start_idx + sequence_length)
            updated_recording = replace(
                recording,
                pass_events=[pass_event],
                input_data=replace(
                    recording.input_data,
                    user_dominant_foot_positions=recording.input_data.user_dominant_foot_positions[start_idx:start_idx + sequence_length],
                    user_non_dominant_foot_positions=recording.input_data.user_non_dominant_foot_positions[start_idx:start_idx + sequence_length],
                    timestamps=recording.input_data.timestamps[start_idx:start_idx + sequence_length],
                ),
                number_of_frames=sequence_length,
                duration=recording.input_data.timestamps[start_idx + sequence_length - 1] - recording.input_data.timestamps[start_idx],

            )
            sample = Sample(recording=updated_recording, pass_event=pass_event)
            samples.append(sample)

        return samples

    @staticmethod
    def _get_pass_event_in_sequence(recording: Recording, start_idx: int, end_idx: int) -> Optional[PassEvent]:
        """Retrieve the pass event in a specific sequence range."""
        for event in recording.pass_events:
            if start_idx <= event.frame_number < end_idx:
                return PassEvent(
                    is_a_pass=True,
                    frame_number=event.frame_number,
                    foot=event.foot,
                    pass_id=event.pass_id,
                    timestamp=event.timestamp,
                )

        return PassEvent(
            is_a_pass=False,
            frame_number=0,
            foot=Foot.UNASSIGNED,
            pass_id=0,
            timestamp=0,
        )
