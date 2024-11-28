from typing import List

from src.domain.inferences import Feature, InputData


class ZeroedPositionDominantFootZ(Feature):

    def calculate(self, input_data: InputData) -> List[float]:
        dominant_positions = input_data.user_dominant_foot_positions
        origin = dominant_positions[0]
        values = [
            p.z - origin.z for p in dominant_positions
        ]

        return values
