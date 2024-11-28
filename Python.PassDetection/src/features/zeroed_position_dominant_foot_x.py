from src.domain.inferences import Feature, Input
from src.domain.recordings import InputData


class ZeroedPositionDominantFootX(Feature):

    def calculate(self, input_data: InputData) -> Input:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        values = [
            p.x - origin.x for p in dominant_positions
        ]
        return Input(self.name, values)
