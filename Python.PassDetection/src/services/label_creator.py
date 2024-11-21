from typing import List, Tuple, Optional

from src.domain.passes import Pass, NoPass
from src.domain.recordings import Recording
from src.domain.samples import Sample


class LabelCreator:

    @staticmethod
    def generate(recordings: List[Recording], sequence_length=10) -> List[Sample]:
        dataset = []
        for trial in recordings:
            labeled_sequences = LabelCreator._create_sequences(trial, sequence_length)
            dataset.extend(labeled_sequences)
        return dataset

    @staticmethod
    def _create_sequences(recording: Recording, sequence_length: int) -> List[Sample]:
        sequences = []
        total_samples = len(recording.user_dominant_foot_positions)

        for start_idx in range(total_samples - sequence_length + 1):
            end_idx = start_idx + sequence_length
            is_a_pass, pass_event_index, pass_timestamp = LabelCreator._get_pass_event_in_sequence(recording, start_idx,
                                                                                                   end_idx)

            if is_a_pass:
                pass_info = Pass(is_a_pass, pass_event_index, pass_timestamp)
            else:
                pass_info = NoPass()

            labeled_trial = Sample(
                frame_rate_hz=recording.frame_rate_hz,
                number_of_frames=sequence_length,
                timestamps=recording.timestamps[start_idx:end_idx],
                trial_number=recording.trial_number,
                duration=recording.duration,
                user_dominant_foot_positions=recording.user_dominant_foot_positions[start_idx:end_idx],
                user_non_dominant_foot_positions=recording.user_non_dominant_foot_positions[start_idx:end_idx],
                pass_info=pass_info
            )
            sequences.append(labeled_trial)
        return sequences

    @staticmethod
    def _get_pass_event_in_sequence(trial: Recording, start_idx: int, end_idx: int) -> Tuple[
        bool, Optional[int], Optional[float]]:
        is_a_pass = False
        pass_event_index = None
        pass_event_timestamp = None

        for event in trial.pass_events:
            if start_idx <= event.frame_number < end_idx:
                is_a_pass = True
                pass_event_index = event.frame_number
                pass_event_timestamp = trial.timestamps[event.frame_number]
                break  # Assuming only one pass event per sequence

        return is_a_pass, pass_event_index, pass_event_timestamp
