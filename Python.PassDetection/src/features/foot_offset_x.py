from src.domain.inferences import Feature, Input
from src.domain.recordings import InputData


class FootOffsetX(Feature):

    def calculate(self, input_data: InputData) -> Input:
        dominant_positions = input_data.user_dominant_foot_positions
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        values = [
            non_dominant_positions[i].x - dominant_positions[i].x
            for i in range(len(dominant_positions))
        ]

        return Input(self.name, values)
