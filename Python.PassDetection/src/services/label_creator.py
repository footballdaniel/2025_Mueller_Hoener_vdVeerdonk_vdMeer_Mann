from dataclasses import replace
from typing import Iterator

from src.domain.recordings import Recording  # Assuming Event is the base class for events
from src.domain.samples import Sample


class LabelCreator:
    def generate(
        self, recording: Recording, sequence_length: int = 10, start_id: int = 0
    ) -> Iterator[Sample]:
        total_frames = len(recording.input_data.user_dominant_foot_positions)
        current_id = start_id

        for event in recording.events:
            event_idx = event.frame_number  # Index of the event

            # Generate windows where the event shifts from last to first index
            for offset in range(sequence_length):
                start_idx = event_idx - sequence_length + 1 + offset
                end_idx = start_idx + sequence_length

                # Ensure indices are within valid range
                if start_idx < 0 or end_idx > total_frames:
                    continue  # Skip if the window is invalid

                # Position of the event within the sequence
                event_pos_in_sequence = event_idx - start_idx

                # Extract the subsequence
                subsequence_dominant = recording.input_data.user_dominant_foot_positions[
                    start_idx:end_idx
                ]
                subsequence_non_dominant = recording.input_data.user_non_dominant_foot_positions[
                    start_idx:end_idx
                ]
                subsequence_timestamps = recording.input_data.timestamps[start_idx:end_idx]

                # Adjust the event frame number relative to the subsequence
                adjusted_event = replace(
                    event, frame_number=event_pos_in_sequence
                )

                # **Implemented TODO**:
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
                    input_data=replace(
                        recording.input_data,
                        user_dominant_foot_positions=subsequence_dominant,
                        user_non_dominant_foot_positions=subsequence_non_dominant,
                        timestamps=subsequence_timestamps,
                    ),
                    number_of_frames=sequence_length,
                    duration=subsequence_timestamps[-1] - subsequence_timestamps[0],
                )

                # Create the sample
                sample = Sample(
                    id=current_id,
                    recording=updated_recording,
                    event=adjusted_event,  # Assuming pass_event can be any event
                )
                current_id += 1
                yield sample
