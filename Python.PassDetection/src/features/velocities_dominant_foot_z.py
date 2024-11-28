from src.domain.inferences import Feature, Input
from src.domain.recordings import InputData


class VelocityDominantFootZ(Feature):
    def calculate(self, input_data: InputData) -> Input:
        dominant_positions = input_data.user_dominant_foot_positions
        timestamps = input_data.timestamps
        values = []

        for i in range(1, len(dominant_positions)):
            dt = timestamps[i] - timestamps[i - 1]
            if dt == 0:
                dt = 1e-6
            dz = (dominant_positions[i].z - dominant_positions[i - 1].z) / dt
            values.append(dz)

        values.insert(0, 0)  # Insert a zero velocity at the start

        return Input(self.name, values)