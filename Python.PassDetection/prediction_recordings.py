import onnxruntime as ort
import torch

from src.domain.recordings import InputData
from src.features.foot_offset import FootOffset
from src.features.velocities_dominant_foot import VelocitiesDominantFoot
from src.features.velocities_non_dominant_foot import VelocitiesNonDominantFoot
from src.features.zeroed_position_dominant_foot import ZeroedPositionDominantFoot
from src.services.recording_parser import RecordingParser

# Load the ONNX model
onnx_model_path = 'pass_detection_model.onnx'
onnx_session = ort.InferenceSession(onnx_model_path)

# Load data samples
parser = RecordingParser()
parser.read_recording_from_json('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.json')
parser.read_pass_events_from_csv('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.csv')

recording = parser.recording

desired_timestamp = 0.1

for timestamp in recording.input_data.timestamps:
    timeseries_length = 10

    start_idx = recording.input_data.timestamps.index(timestamp)
    end_idx = start_idx + timeseries_length

    if end_idx >= len(recording.input_data.timestamps):
        break

    dominant_foot_positions = recording.input_data.user_dominant_foot_positions[start_idx:end_idx]
    non_dominant_foot_positions = recording.input_data.user_non_dominant_foot_positions[start_idx:end_idx]
    timestamps = recording.input_data.timestamps[start_idx:end_idx]

    input_data = InputData(
        user_dominant_foot_positions=dominant_foot_positions,
        user_non_dominant_foot_positions=non_dominant_foot_positions,
        timestamps=timestamps
    )

    zeroed_position = ZeroedPositionDominantFoot()
    foot_offset = FootOffset()
    velocities_dominant_foot = VelocitiesDominantFoot()
    velocities_non_dominant_foot = VelocitiesNonDominantFoot()

    features = []
    features.extend(zeroed_position.calculate(input_data))
    features.extend(foot_offset.calculate(input_data))
    features.extend(velocities_dominant_foot.calculate(input_data))
    features.extend(velocities_non_dominant_foot.calculate(input_data))

    batch_size = 1
    timeseries_length = 10
    features_count = len(features)

    flattened_values = []
    for feature in features:
        flattened_values.extend(feature.values)

    input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
    input_tensor = input_tensor.view(batch_size, timeseries_length, features_count)

    input_numpy = input_tensor.cpu().numpy()
    onnx_inputs = {onnx_session.get_inputs()[0].name: input_numpy}
    onnx_outputs = onnx_session.run(None, onnx_inputs)

    probability = onnx_outputs[0].squeeze()
    print(round(float(probability),2))

    if abs(timestamp - desired_timestamp) < 0.01:
        flat_input = input_tensor.flatten().cpu().numpy()

        with open('input_tensor.txt', 'w') as f:
            for value in flat_input:
                f.write(f"{value:.3f}\n")
