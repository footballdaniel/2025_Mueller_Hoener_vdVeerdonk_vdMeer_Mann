from typing import List

from src.domain import AugmentedLabeledSample, Sample


class Augmentor:

    @staticmethod
    def augment(trials: List[Sample]) -> List[AugmentedLabeledSample]:
        rotation_angles = [angle for angle in range(5, 360, 10)]
        augmented_trials: List[AugmentedLabeledSample] = []

        for trial in trials:
            # Original trial without modifications
            augmented_trials.append(AugmentedLabeledSample(
                **trial.__dict__,
                rotation_angle=0
            ))

            if trial.is_a_pass:
                Augmentor.add_rotated_passes(augmented_trials, trial, rotation_angles)
                Augmentor.add_swapped_feet_pass(augmented_trials, trial)
                Augmentor.add_swapped_and_rotated_passes(augmented_trials, trial, rotation_angles)

        return augmented_trials

    @staticmethod
    def add_rotated_passes(augmented_trials: List[AugmentedLabeledSample], trial: Sample,
                           rotation_angles: List[int]):
        """Adds rotated versions of the original trial."""
        for angle in rotation_angles:
            rotated_trial = Sample.rotate_around_y(trial, angle)
            augmented_trials.append(AugmentedLabeledSample(
                **rotated_trial.__dict__,
                rotation_angle=angle
            ))

    @staticmethod
    def add_swapped_feet_pass(augmented_trials: List[AugmentedLabeledSample], trial: Sample):
        """Adds a swapped feet version of the original trial."""
        swapped_trial = Sample.swap_feet(trial)
        augmented_trials.append(AugmentedLabeledSample(
            **swapped_trial.__dict__,
            rotation_angle=0  # No rotation on swapped
        ))

    @staticmethod
    def add_swapped_and_rotated_passes(augmented_trials: List[AugmentedLabeledSample], trial: Sample,
                                       rotation_angles: List[int]):
        """Adds swapped and rotated versions of the trial."""
        swapped_trial = Sample.swap_feet(trial)
        for angle in rotation_angles:
            swapped_rotated_trial = Sample.rotate_around_y(swapped_trial, angle)
            augmented_trials.append(AugmentedLabeledSample(
                **swapped_rotated_trial.__dict__,
                rotation_angle=angle
            ))
