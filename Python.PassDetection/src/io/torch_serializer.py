from dataclasses import asdict
from typing import List

import torch

from src.features import PositionAndVelocityFeature


def serialize_data(features: List[PositionAndVelocityFeature], file_path: str):
    data = [asdict(feature) for feature in features]
    torch.save(data, file_path)


def deserialize_data(file_path: str) -> List[PositionAndVelocityFeature]:
    data = torch.load(file_path)
    return [PositionAndVelocityFeature(**item) for item in data]
