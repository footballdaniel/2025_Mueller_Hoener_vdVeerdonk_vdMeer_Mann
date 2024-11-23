import pickle
from typing import List

import onnxruntime as ort
import torch

from src.domain.samples import Sample

# Load ONNX model
onnx_model_path = 'pass_detection_model.onnx'
onnx_session = ort.InferenceSession(onnx_model_path)

# Load data samples
with open('dataset.pkl', 'rb') as f:
    samples: List[Sample] = pickle.load(f)

# Loop through each sample
for sample in samples:
    # Prepare the input tensor
    batch_size = 1
    timeseries_length = 10
    features = sample.inference.targets
    features_count = len(features)

    flattened_values = [value for feature in features for value in feature.values]
    input_tensor = torch.tensor(flattened_values, dtype=torch.float32)
    input_tensor = input_tensor.view(batch_size, timeseries_length, features_count)

    # Convert tensor to numpy and run ONNX inference
    input_numpy = input_tensor.cpu().numpy()
    onnx_inputs = {onnx_session.get_inputs()[0].name: input_numpy}
    onnx_outputs = onnx_session.run(None, onnx_inputs)

    # Extract predicted probability
    predicted_probability = float(onnx_outputs[0].squeeze())

    # Print results
    print(
        f"Trial {sample.recording.trial_number}, Timestamp: {sample.recording.input_data.timestamps[0]:.3f} - "
        f"Predicted Probability: {predicted_probability:.3f}, "
        f"Actual Probability: {sample.inference.pass_probability:.3f}"
    )
