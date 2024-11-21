from typing import List

from src.domain import AugmentedLabeledSample, FeatureCalculator, SampleWithFeatures


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators = []

    def add_feature(self, feature_calculator: FeatureCalculator):
        self.feature_calculators.append(feature_calculator)

    def engineer_features(self, samples: List[AugmentedLabeledSample]) -> List[SampleWithFeatures]:
        calculated_features_list = []
        for sample in samples:
            features = []
            for calculator in self.feature_calculators:
                calculated = calculator.calculate(sample)
                features.extend(calculated)  # Each calculator returns a list of Features

            outcome = int(sample.is_a_pass)
            calculated_features_list.append(SampleWithFeatures(**sample.__dict__, features=features, output=outcome))
        return calculated_features_list

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)
