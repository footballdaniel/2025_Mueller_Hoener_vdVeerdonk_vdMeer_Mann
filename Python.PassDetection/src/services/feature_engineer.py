from typing import List

from src.domain.inferences import Feature, InputData, Shape


class FeatureEngineer:
    def __init__(self):
        self.data_shape = Shape()
        self.features: List[Feature] = []

    def add_feature(self, feature_calculator: Feature):
        self.features.append(feature_calculator)

    def engineer(self, input_data: InputData) -> List[float]:
        inputs = []

        for feature in self.features:
            inputs.extend(feature.calculate(input_data))

        self.data_shape = Shape(len(input_data.timestamps), len(self.features))
        return inputs

    @property
    def shape(self) -> Shape:
        return self.data_shape

    @property
    def input_size(self) -> int:
        return len(self.features)
