from typing import List

from src.domain import AugmentedLabeledSample, FeatureCalculator, CalculatedSample


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators = []

    def add_feature(self, feature_calculator: FeatureCalculator):
        self.feature_calculators.append(feature_calculator)

    def engineer_features(self, trials: List[AugmentedLabeledSample]) -> List[CalculatedSample]:
        calculated_features_list = []
        for trial in trials:
            features = []
            for calculator in self.feature_calculators:
                calculated = calculator.calculate(trial)
                features.extend(calculated)  # Each calculator returns a list of Features

            outcome = int(trial.is_a_pass)
            calculated_features_list.append(CalculatedSample(features=features, outcome=outcome))
        return calculated_features_list

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)
