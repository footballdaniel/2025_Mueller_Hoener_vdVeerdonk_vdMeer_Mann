from dataclasses import replace
from typing import List

from src.domain.inferences import FeatureCalculator, Inference
from src.domain.samples import Sample


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators = []

    def add_feature(self, feature_calculator: FeatureCalculator):
        self.feature_calculators.append(feature_calculator)

    def engineer_features(self, samples: List[Sample]) -> List[Sample]:
        engineered_samples = []
        for sample in samples:
            features = []
            for calculator in self.feature_calculators:
                calculated = calculator.calculate(sample.recording)
                features.extend(calculated)  # Each calculator returns a list of Features

            outcome = int(sample.pass_event.is_a_pass)
            inference = Inference(features, outcome)
            engineered_samples.append(replace(sample, inference=inference))
        return engineered_samples

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)
