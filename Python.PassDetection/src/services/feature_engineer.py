from typing import List, Tuple

from src.domain.inferences import Feature, ComputedFeatures
from src.domain.recordings import InputData


class FeatureEngineer:
    def __init__(self):
        self.features: List[Feature] = []

    def add_feature(self, feature_calculator: Feature):
        self.features.append(feature_calculator)

    def engineer(self, input_data: InputData) -> ComputedFeatures:

        inputs = []
        for feature in self.features:
            inputs.append(feature.calculate(input_data))

        flattened_values = []
        for input in inputs:
            flattened_values.extend(input.values)

        desired_shape = len(inputs[0].values), len(self.features)
        engineered_input_data = ComputedFeatures(flattened_values, desired_shape, inputs)
        return engineered_input_data

    @property
    def input_size(self) -> int:
        return len(self.features)
