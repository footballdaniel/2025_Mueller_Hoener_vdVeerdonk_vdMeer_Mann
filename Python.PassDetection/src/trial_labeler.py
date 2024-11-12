from typing import List, Tuple, Optional

from src.domain import Trial, Sample


class Labels:

    @staticmethod
    def generate(trials: List[Trial], sequence_length=10) -> List[Sample]:
        dataset = []
        for trial in trials:
            labeled_sequences = Labels._create_sequences(trial, sequence_length)
            dataset.extend(labeled_sequences)
        return dataset

    @staticmethod
    def _create_sequences(trial: Trial, sequence_length: int) -> List[Sample]:
        sequences = []
        total_samples = len(trial.user_dominant_foot_positions)

        for start_idx in range(total_samples - sequence_length + 1):
            end_idx = start_idx + sequence_length
            is_a_pass, pass_event_index = Labels._get_pass_event_in_sequence(trial, start_idx, end_idx)

            labeled_trial = Sample(
                frame_rate_hz=trial.frame_rate_hz,
                number_of_frames=sequence_length,
                timestamps=trial.timestamps[start_idx:end_idx],
                trial_number=trial.trial_number,
                duration=trial.duration,
                user_dominant_foot_positions=trial.user_dominant_foot_positions[start_idx:end_idx],
                user_non_dominant_foot_positions=trial.user_non_dominant_foot_positions[start_idx:end_idx],
                is_a_pass=is_a_pass,
                pass_id=pass_event_index
            )
            sequences.append(labeled_trial)
        return sequences

    @staticmethod
    def _get_pass_event_in_sequence(trial: Trial, start_idx: int, end_idx: int) -> Tuple[bool, Optional[int]]:
        is_a_pass = False
        pass_event_index = None

        for event in trial.pass_events:
            if start_idx <= event.frame_number < end_idx:
                is_a_pass = True
                pass_event_index = event.frame_number
                break  # Assuming only one pass event per sequence

        return is_a_pass, pass_event_index
