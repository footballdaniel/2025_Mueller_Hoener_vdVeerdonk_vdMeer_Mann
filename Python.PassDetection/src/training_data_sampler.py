from src.domain import Position
from src.features import PositionAndVelocityFeature


class DataSampler:
    def __init__(self, trials, sequence_length=10):
        self.trials = trials
        self.sequence_length = sequence_length

    def generate_dataset(self):
        dataset = []
        for trial in self.trials:
            sequences = self._create_sequences(trial)
            dataset.extend(sequences)
        return dataset

    def _create_sequences(self, trial):
        sequences = []
        total_samples = len(trial.user_dominant_foot_positions)
        for i in range(total_samples - self.sequence_length + 1):
            seq = self._extract_features(trial, i)
            sequences.append(seq)
        return sequences

    def _extract_features(self, trial, start_idx):
        end_idx = start_idx + self.sequence_length

        # Extract positions
        dominant_foot_positions = trial.user_dominant_foot_positions[start_idx:end_idx]
        non_dominant_foot_positions = trial.user_non_dominant_foot_positions[start_idx:end_idx]

        # Extract timestamps
        timestamps = trial.timestamps[start_idx:end_idx]

        # Zero positions
        zeroed_dominant_foot_positions = self._zero_positions(dominant_foot_positions)

        # Calculate offsets
        offset_dominant_foot_to_non_dominant_foot = self._calculate_offsets(
            dominant_foot_positions, non_dominant_foot_positions
        )

        # Calculate velocities
        velocities_dominant_foot = self._calculate_velocities(dominant_foot_positions, timestamps)
        velocities_non_dominant_foot = self._calculate_velocities(non_dominant_foot_positions, timestamps)

        # Determine if there is a pass event in the current sequence
        pass_event_info = self._get_pass_event_in_sequence(trial, start_idx, end_idx, timestamps)

        feature = PositionAndVelocityFeature(
            is_a_pass=pass_event_info['is_a_pass'],
            zeroed_position_dominant_foot=zeroed_dominant_foot_positions,
            offset_dominant_foot_to_non_dominant_foot=offset_dominant_foot_to_non_dominant_foot,
            velocities_dominant_foot=velocities_dominant_foot,
            velocities_non_dominant_foot=velocities_non_dominant_foot,
            trial_number=trial.trial_number,
            timestamps=timestamps,
            start_time=round(timestamps[0],2),
            pass_event_timestamp=pass_event_info['pass_event_timestamp'],
            pass_event_index=pass_event_info['pass_event_index']
        )
        return feature

    @staticmethod
    def _zero_positions(positions):
        origin = positions[0]
        return [Position(p.x - origin.x, p.y - origin.y, p.z - origin.z) for p in positions]

    @staticmethod
    def _calculate_offsets(dominant_positions, non_dominant_positions):
        return [
            Position(
                non_dominant_positions[i].x - dominant_positions[i].x,
                non_dominant_positions[i].y - dominant_positions[i].y,
                non_dominant_positions[i].z - dominant_positions[i].z,
            )
            for i in range(len(dominant_positions))
        ]

    @staticmethod
    def _calculate_velocities(positions, timestamps):
        velocities = []
        for i in range(1, len(positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6  # Avoid division by zero
            dx = (positions[i].x - positions[i - 1].x) / dt
            dy = (positions[i].y - positions[i - 1].y) / dt
            dz = (positions[i].z - positions[i - 1].z) / dt
            velocities.append(Position(dx, dy, dz))
        # For the first position, velocity is zero
        velocities.insert(0, Position(0, 0, 0))
        return velocities

    @staticmethod
    def _get_pass_event_in_sequence(trial, start_idx, end_idx, timestamps):
        is_a_pass = False
        pass_event_timestamp = None
        pass_event_index = None

        for event in trial.pass_events:
            if start_idx <= event.frame_number < end_idx:
                is_a_pass = True
                pass_event_index = event.frame_number - start_idx
                pass_event_timestamp = timestamps[pass_event_index]
                break  # Assuming only one pass event per sequence

        return {
            'is_a_pass': is_a_pass,
            'pass_event_timestamp': pass_event_timestamp,
            'pass_event_index': pass_event_index,
        }
