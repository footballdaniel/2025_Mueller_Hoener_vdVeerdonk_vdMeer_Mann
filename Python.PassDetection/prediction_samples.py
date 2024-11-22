import pickle
from typing import List

import onnxruntime as ort
import torch

from src.domain.samples import Sample

# Load the ONNX model
onnx_model_path = 'pass_detection_model.onnx'
onnx_session = ort.InferenceSession(onnx_model_path)

# Load data samples
with open('dataset.pkl', 'rb') as f:
    samples: List[Sample] = pickle.load(f)


def predict_probability_onnx(features):
    features_as_tensors = [feature.to_tensor() for feature in features]
    input_tensor = torch.stack(features_as_tensors, dim=1).unsqueeze(0)  # Add batch dimension

    # Flatten the tensor
    # flat_input = input_tensor.flatten().cpu().numpy()

    # input_tensor.cpu()
    # import pandas as pd
    # input_tensor_df = pd.DataFrame(input_tensor.squeeze(0).cpu().numpy()).round(2)
    # input_tensor_df.to_csv('input_tensor.csv', index=False)

    input_numpy = input_tensor.cpu().numpy()
    onnx_inputs = {onnx_session.get_inputs()[0].name: input_numpy}
    onnx_outputs = onnx_session.run(None, onnx_inputs)

    probability = onnx_outputs[0].squeeze()
    return float(probability)


for sample in samples:
    predicted_probability = predict_probability_onnx(sample.inference.features)
    print(f"Sample {sample.recording.trial_number} - Predicted Probability: {predicted_probability:.3f}, Actual: {sample.inference.pass_probability}")
