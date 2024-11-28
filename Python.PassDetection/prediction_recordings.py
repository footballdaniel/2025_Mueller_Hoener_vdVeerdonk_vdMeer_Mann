import json
from pathlib import Path

import onnxruntime as ort
import torch

from src.domain.inferences import InputData
from src.features.feature_registry import FeatureRegistry
from src.services.feature_engineer import FeatureEngineer
from src.services.recording_parser import RecordingParser

# Load the ONNX model
onnx_model_path = 'pass_detection_model.onnx'
onnx_session = ort.InferenceSession(onnx_model_path)
feature_names = json.loads(onnx_session.get_modelmeta().custom_metadata_map['features'])

"""LOAD RECORDING"""
timeseries_file = Path('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.json')
events_file = Path('../Data/Pilot_4/trial_3_2024-10-29_15-58-40.csv')
recording = RecordingParser.parse_recording(timeseries_file, events_file)

"""FEATURE ENGINEER"""
engineer = FeatureEngineer()

for feature_name in feature_names:
    feature = FeatureRegistry.create(feature_name)
    engineer.add_feature(feature)

"""PREDICTION"""
timestamps = []
predictions = []

for timestamp in recording.timestamps:
    timeseries_length = 10

    start_idx = recording.timestamps.index(timestamp)
    end_idx = start_idx + timeseries_length

    if end_idx >= len(recording.timestamps):
        break

    dominant_foot_positions = recording.user_dominant_foot_positions[start_idx:end_idx]
    non_dominant_foot_positions = recording.user_non_dominant_foot_positions[start_idx:end_idx]
    sliced_timestamps = recording.timestamps[start_idx:end_idx]

    input_data = InputData(
        dominant_foot_positions,
        non_dominant_foot_positions,
        sliced_timestamps,  # Use the sliced timestamps here
    )

    flattened_input = engineer.engineer(input_data)

    batch_size = 1

    input_tensor = torch.tensor(flattened_input, dtype=torch.float32)
    input_tensor = input_tensor.view(batch_size, timeseries_length, engineer.feature_size)

    input_numpy = input_tensor.cpu().numpy()
    onnx_inputs = {onnx_session.get_inputs()[0].name: input_numpy}
    onnx_outputs = onnx_session.run(None, onnx_inputs)
    probability = onnx_outputs[0].squeeze()

    predictions.append(probability)
    timestamps.append(timestamp)  # Append the current timestamp here

    print(timestamp)

output_file = Path('predictions.csv')
with open(output_file, 'w') as f:
    f.write('timestamp,probability\n')
    for timestamp, probability in zip(timestamps, predictions):
        formatted_probability = f"{probability:.2f}"  # Round to 2 decimals and add leading zeroes
        formatted_timestamp = f"{timestamp:.2f}"  # Assuming timestamps should also be formatted similarly
        f.write(f'{formatted_timestamp},{formatted_probability}\n')