from typing import List, Tuple

from src.domain.inferences import Feature, EngineeredInputData
from src.domain.recordings import InputData


class FeatureEngineer:
    def __init__(self):
        self.features: List[Feature] = []

    def add_feature(self, feature_calculator: Feature):
        self.features.append(feature_calculator)

    def engineer(self, input_data: InputData) -> EngineeredInputData:
        flattened_values = []
        for feature in self.features:
            feature.calculate(input_data)
            flattened_values.extend(feature.values)

        outcome = int(input_data.is_pass)

        desired_shape = len(self.features[0].values), len(self.features)
        engineered_input_data = EngineeredInputData(flattened_values, outcome, desired_shape, self.features)
        return engineered_input_data

    @property
    def input_size(self) -> int:
        return len(self.features)
