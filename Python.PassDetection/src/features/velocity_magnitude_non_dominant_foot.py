from src.domain.inferences import Feature, Input
from src.domain.recordings import InputData


class VelocityMagnitudeNonDominantFoot(Feature):

    def calculate(self, input_data: InputData) -> Input:
        non_dominant_positions = input_data.user_non_dominant_foot_positions
        timestamps = input_data.timestamps
        values = []

        for i in range(1, len(non_dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6
            dx = (non_dominant_positions[i].x - non_dominant_positions[i - 1].x) / dt
            dy = (non_dominant_positions[i].y - non_dominant_positions[i - 1].y) / dt
            dz = (non_dominant_positions[i].z - non_dominant_positions[i - 1].z) / dt
            magnitude = (dx ** 2 + dy ** 2 + dz ** 2) ** 0.5
            values.append(magnitude)

        values.insert(0, 0.0)  # Insert a zero magnitude at the start

        return Input(self.name, values)