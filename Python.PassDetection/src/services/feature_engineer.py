from typing import List, Tuple

from src.domain.inferences import BaseFeature
from src.domain.recordings import InputData, EngineeredInputData


class FeatureEngineer:
    def __init__(self):
        self.feature_calculators: List[BaseFeature] = []

    def add_feature(self, feature_calculator: BaseFeature):
        self.feature_calculators.append(feature_calculator)

    def engineer(self, input_data: InputData) -> EngineeredInputData:
        targets = []
        for calculator in self.feature_calculators:
            calculated = calculator.calculate(input_data)
            targets.extend(calculated)  # Each calculator returns a list of Features

        flattened_values = []
        for target in targets:
            flattened_values.extend(target.values)

        outcome = int(input_data.is_pass)

        desired_shape = len(targets[0].values), len(targets)
        engineered_input_data = EngineeredInputData(flattened_values, outcome, desired_shape)
        return engineered_input_data

    @property
    def input_size(self) -> int:
        # Sum the sizes of each feature calculator to get the input size
        return sum(feature.size for feature in self.feature_calculators)
