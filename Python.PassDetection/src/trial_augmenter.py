from typing import List

from src.domain import AugmentedLabeledTrial, LabeledTrial


class TrialAugmenter:

    @staticmethod
    def augment(trials: List[LabeledTrial]) -> List[AugmentedLabeledTrial]:
        rotation_angles = [angle for angle in range(5, 360, 10)]
        augmented_trials: List[AugmentedLabeledTrial] = []

        for trial in trials:
            augmented_trials.append(AugmentedLabeledTrial(
                **trial.__dict__,
                rotation_angle=0
            ))

            if trial.is_a_pass:
                TrialAugmenter.add_rotated_passes(augmented_trials, trial, rotation_angles)
                TrialAugmenter.add_swapped_feet_pass(augmented_trials, trial)
                TrialAugmenter.add_swapped_and_rotated_passes(augmented_trials, trial, rotation_angles)

        return augmented_trials

    @staticmethod
    def add_rotated_passes(augmented_trials: List[AugmentedLabeledTrial], trial: LabeledTrial,
                           rotation_angles: List[int]):
        """Adds rotated versions of the original trial."""
        for angle in rotation_angles:
            rotated_trial = trial.rotate_around_y(angle)
            augmented_trials.append(AugmentedLabeledTrial(
                **rotated_trial.__dict__,
                rotation_angle=angle
            ))

    @staticmethod
    def add_swapped_feet_pass(augmented_trials: List[AugmentedLabeledTrial], trial: LabeledTrial):
        """Adds a swapped feet version of the original trial."""
        swapped_trial = trial.swap_feet()
        augmented_trials.append(AugmentedLabeledTrial(
            **swapped_trial.__dict__,
            rotation_angle=0  # No rotation on swapped
        ))

    @staticmethod
    def add_swapped_and_rotated_passes(augmented_trials: List[AugmentedLabeledTrial], trial: LabeledTrial,
                                       rotation_angles: List[int]):
        """Adds swapped and rotated versions of the trial."""
        swapped_trial = trial.swap_feet()
        for angle in rotation_angles:
            swapped_rotated_trial = swapped_trial.rotate_around_y(angle)
            augmented_trials.append(AugmentedLabeledTrial(
                **swapped_rotated_trial.__dict__,
                rotation_angle=angle
            ))
