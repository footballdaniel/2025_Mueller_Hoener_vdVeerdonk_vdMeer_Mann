from __future__ import annotations

from dataclasses import dataclass, replace, field
from pathlib import Path
from typing import Optional

from src.domain.augmentations import Augmentation, NoAugmentation
from src.domain.inferences import Inference, NoInference
from src.domain.recordings import Recording, PassEvent


class IngestableRecording:
    def __init__(self):
        self.event_file: Optional[Path] = None
        self.timeseries_file: Optional[Path] = None
        self.stem: str = str()

    def add_event_file(self, file: Path) -> None:
        self.event_file = file
        self.stem = file.stem

    def add_timeseries_file(self, file: Path) -> None:
        self.timeseries_file = file
        self.stem = file.stem

    def both_files_present(self) -> bool:
        return self.event_file is not None and self.timeseries_file is not None

    def __eq__(self, other: IngestableRecording) -> bool:
        return self.stem == other.stem

    def __hash__(self):
        return hash(self.stem)


@dataclass
class Sample:
    id: int
    recording: Recording
    pass_event: PassEvent
    inference: Inference = field(default_factory=NoInference)
    augmentation: Augmentation = field(default_factory=NoAugmentation)

    def mirror(self) -> Sample:
        """Return a mirrored version of the sample."""
        new_recording = replace(
            self.recording,
            input_data=replace(
                self.recording.input_data,
                user_dominant_foot_positions=[pos.mirror_x() for pos in
                                              self.recording.input_data.user_dominant_foot_positions],
                user_non_dominant_foot_positions=[pos.mirror_x() for pos in
                                                  self.recording.input_data.user_non_dominant_foot_positions],
            ),
        )
        return replace(
            self,
            recording=new_recording,
            augmentation=replace(
                self.augmentation,
                swapped_feet=True
            ),
        )

    def rotate_around_y(self, angle_degrees: float) -> Sample:
        new_recording = replace(
            self.recording,
            input_data=replace(
                self.recording.input_data,
                user_dominant_foot_positions=[
                    pos.rotate_around_y(angle_degrees) for pos in self.recording.input_data.user_dominant_foot_positions
                ],
                user_non_dominant_foot_positions=[
                    pos.rotate_around_y(angle_degrees) for pos in
                    self.recording.input_data.user_non_dominant_foot_positions
                ],
            ),
        )
        return replace(
            self,
            recording=new_recording,
            augmentation=replace(
                self.augmentation,
                rotation_angle=angle_degrees
            ),
        )

    def swap_feet(self) -> Sample:
        """Return a version of the sample with dominant and non-dominant foot positions swapped."""
        new_recording = replace(
            self.recording,
            input_data=replace(
                self.recording.input_data,
                user_dominant_foot_positions=self.recording.input_data.user_non_dominant_foot_positions.copy(),
                user_non_dominant_foot_positions=self.recording.input_data.user_dominant_foot_positions.copy(),
            ),
        )
        return replace(
            self,
            recording=new_recording,
            augmentation=replace(
                self.augmentation,
                swapped_feet=True
            ),
        )
