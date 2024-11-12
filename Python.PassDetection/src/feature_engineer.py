from typing import List

from src.domain import FeatureCalculator, AugmentedLabeledTrial, CalculatedFeatures


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators = []

    def add_feature(self, feature_calculator: FeatureCalculator):
        self.feature_calculators.append(feature_calculator)

    def engineer_features(self, trials: List[AugmentedLabeledTrial]) -> List[CalculatedFeatures]:
        calculated_features_list = []
        for trial in trials:
            features = []
            for calculator in self.feature_calculators:
                calculated = calculator.calculate(trial)
                features.extend(calculated)  # Each calculator returns a list of Features

            outcome = int(trial.is_a_pass)
            calculated_features_list.append(CalculatedFeatures(features=features, outcome=outcome))
        return calculated_features_list
