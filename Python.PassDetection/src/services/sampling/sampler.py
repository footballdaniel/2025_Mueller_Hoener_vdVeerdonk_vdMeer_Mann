from dataclasses import replace
from typing import Iterator

from src.domain.samples import Sample, Recording, Event


class Sampler:
    @staticmethod
    def generate(
            recording: Recording,
            sequence_length: int = 10,
            start_id: int = 0
    ) -> Iterator[Sample]:
        total_frames = len(recording.user_dominant_foot_positions)
        current_id = start_id

        for event in recording.events:
            event_idx = event.frame_number  # Index of the event

            # Generate windows where the event shifts from last to first index
            dont_export_event_when_its_on_last_frame_offset = 1
            export_only_timeseries_where_event_is_on_last_half_of_sequence = sequence_length

            for offset in range(export_only_timeseries_where_event_is_on_last_half_of_sequence):
                start_idx = event_idx - sequence_length + dont_export_event_when_its_on_last_frame_offset + offset
                end_idx = start_idx + sequence_length

                # Ensure indices are within valid range
                if start_idx < 0 or end_idx > total_frames:
                    continue  # Skip if the window is invalid

                # Position of the event within the sequence
                event_pos_in_sequence = event_idx - start_idx

                subsequence_dominant = recording.user_dominant_foot_positions[start_idx:end_idx]
                subsequence_non_dominant = recording.user_non_dominant_foot_positions[start_idx:end_idx]
                subsequence_timestamps = recording.timestamps[start_idx:end_idx]

                # Adjust the event frame number relative to the subsequence
                adjusted_event = replace(event, frame_number=event_pos_in_sequence)

                # If the event is not a pass, but there is a pass event in the sequence, then skip
                if not adjusted_event.is_pass:
                    # Collect events in the current window, excluding the current event
                    events_in_window = [
                        e for e in recording.events
                        if start_idx <= e.frame_number < end_idx and e != event
                    ]
                    # Check if there's any pass event in the window
                    if any(e.is_pass for e in events_in_window):
                        continue  # Skip this window

                # Create updated recording with the subsequence
                updated_recording = replace(
                    recording,
                    events=[adjusted_event],
                    user_dominant_foot_positions=subsequence_dominant,
                    user_non_dominant_foot_positions=subsequence_non_dominant,
                    timestamps=subsequence_timestamps,
                    number_of_frames=sequence_length,
                    duration=subsequence_timestamps[-1] - subsequence_timestamps[0]
                )

                sample = Sample(
                    id=current_id,
                    recording=updated_recording,
                )

                yield sample
                current_id += 1  # Increment ID for each sample
