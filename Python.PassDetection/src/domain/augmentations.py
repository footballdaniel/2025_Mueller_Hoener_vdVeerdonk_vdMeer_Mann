from dataclasses import dataclass


@dataclass(frozen=True)
class Augmentation:
    rotation_angle: float = 0.0
    swapped_feet: bool = False


@dataclass(frozen=True)
class NoAugmentation(Augmentation):
    pass
