from dataclasses import dataclass
from itertools import product
from pathlib import Path

import yaml


@dataclass
class Configuration:
    features: list[str]
    model_type: str
    loss_function: str
    learning_rate: float
    epochs: int
    early_stopping_patience: int
    early_stopping_delta: float
    batch_size: int


class ConfigurationParser:
    @staticmethod
    def generate_configurations(yaml_data: Path):
        with yaml_data.open() as f:
            data = yaml.safe_load(f)

        features = data['features']
        hyperparams = data['hyperparameters']

        all_combinations = product(
            features,
            [hyperparams['model_type']],
            [hyperparams['loss_function']],
            [hyperparams['learning_rate']],
            [hyperparams['epochs']],
            [hyperparams['early_stopping_patience']],
            [hyperparams['early_stopping_delta']],
            [hyperparams['batch_size']]
        )

        configurations = [
            Configuration(*combo) for combo in all_combinations
        ]

        return configurations
