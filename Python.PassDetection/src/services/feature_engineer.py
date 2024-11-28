from typing import List

from src.domain.inferences import Feature, InputData


class FeatureEngineer:
    def __init__(self):
        self.features: List[Feature] = []

    def add_feature(self, feature_calculator: Feature):
        self.features.append(feature_calculator)

    def engineer(self, input_data: InputData) -> List[float]:
        inputs = []
        for feature in self.features:
            inputs.extend(feature.calculate(input_data))

        return inputs

    @property
    def feature_size(self) -> int:
        return len(self.features)
