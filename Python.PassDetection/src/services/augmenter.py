from typing import List

from src.domain.samples import Sample


class Augmenter:

    @staticmethod
    def augment(samples: List[Sample], only_augment_passes: bool) -> List[Sample]:
        rotation_angles = [angle for angle in range(5, 360, 10)]
        augmented_samples: List[Sample] = []

        for sample in samples:
            augmented_samples.append(sample)

            if only_augment_passes and not sample.event.is_pass:
                continue

            Augmenter.add_rotated_passes(augmented_samples, sample, rotation_angles)
            Augmenter.add_swapped_feet_pass(augmented_samples, sample)
            Augmenter.add_swapped_and_rotated_passes(augmented_samples, sample, rotation_angles)

        return augmented_samples

    @staticmethod
    def add_rotated_passes(augmented_samples: List[Sample], trial: Sample, rotation_angles: List[int]):
        for angle in rotation_angles:
            rotated_trial = Sample.rotate_around_y(trial, angle)
            augmented_samples.append(rotated_trial)

    @staticmethod
    def add_swapped_feet_pass(augmented_samples: List[Sample], trial: Sample):
        swapped_sample = Sample.swap_feet(trial)
        augmented_samples.append(swapped_sample)

    @staticmethod
    def add_swapped_and_rotated_passes(augmented_samples: List[Sample], trial: Sample, rotation_angles: List[int]):
        swapped_sample = Sample.swap_feet(trial)
        for angle in rotation_angles:
            swapped_and_rotated_trial = Sample.rotate_around_y(swapped_sample, angle)
            augmented_samples.append(swapped_and_rotated_trial)
