from src.domain.inferences import Feature, Input
from src.domain.recordings import InputData


class VelocityMagnitudeDominantFoot(Feature):

    def calculate(self, input_data: InputData) -> Input:
        dominant_positions = input_data.user_dominant_foot_positions
        timestamps = input_data.timestamps
        values = []

        for i in range(1, len(dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6
            dx = (dominant_positions[i].x - dominant_positions[i - 1].x) / dt
            dy = (dominant_positions[i].y - dominant_positions[i - 1].y) / dt
            dz = (dominant_positions[i].z - dominant_positions[i - 1].z) / dt
            magnitude = (dx ** 2 + dy ** 2 + dz ** 2) ** 0.5
            values.append(magnitude)

        values.insert(0, 0.0)  # Insert a zero magnitude at the start
        return Input(self.name, values)
