from dataclasses import replace
from typing import List

from src.domain.inferences import BaseFeature, Inference
from src.domain.samples import Sample


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators: List[BaseFeature] = []

    def add_feature(self, feature_calculator: BaseFeature):
        self.feature_calculators.append(feature_calculator)

    def engineer(self, sample: Sample) -> Sample:
        targets = []
        for calculator in self.feature_calculators:
            calculated = calculator.calculate(sample.recording.input_data)
            targets.extend(calculated)  # Each calculator returns a list of Features

        outcome = int(sample.event.is_pass)

        return replace(
            sample,
            inference=Inference(
                targets,
                outcome,
                split=sample.inference.split
            )
        )

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)

