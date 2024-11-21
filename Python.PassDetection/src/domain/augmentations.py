from dataclasses import dataclass


@dataclass
class Augmentation:
    rotation_angle: float
    swapped_feet: bool


@dataclass
class NoAugmentation(Augmentation):
    rotation_angle: float = 0
    swapped_feet: bool = False
