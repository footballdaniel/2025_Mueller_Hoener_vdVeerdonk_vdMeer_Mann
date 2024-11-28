import json
from pathlib import Path

import onnxruntime as ort
import torch

from src.domain.inferences import InputData
from src.services.feature_engineer import FeatureEngineer
from src.services.recording_parser import RecordingParser

# Load the ONNX model
onnx_model_path = 'pass_detection_model.onnx'
onnx_session = ort.InferenceSession(onnx_model_path)
# read metadata from onnx model
feature_names = json.loads(onnx_session.get_modelmeta().custom_metadata_map['features'])

engineer = FeatureEngineer()

# Load data samples
parser = RecordingParser()
parser.read_recording_from_json(Path('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.json'))
parser.read_pass_events_from_csv(Path('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.csv'))

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
        timestamps=timestamps,
    )

    computed_features = engineer.engineer(recording.input_data)

    batch_size = 1

    input_tensor = torch.tensor(computed_features.flattened_values, dtype=torch.float32)
    input_tensor = input_tensor.view(batch_size, timeseries_length, computed_features.dimensions[1])

    input_numpy = input_tensor.cpu().numpy()
    onnx_inputs = {onnx_session.get_inputs()[0].name: input_numpy}
    onnx_outputs = onnx_session.run(None, onnx_inputs)

    probability = onnx_outputs[0].squeeze()
    print(round(float(probability), 2))

    if abs(timestamp - desired_timestamp) < 0.01:
        flat_input = input_tensor.flatten().cpu().numpy()

        with open('input_tensor.txt', 'w') as f:
            for value in flat_input:
                f.write(f"{value:.3f}\n")
                print(f"{value:.3f}")
