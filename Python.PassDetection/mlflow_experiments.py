"""READING CONFIGURATIONS"""
from random import random

import mlflow

from src.nn.lstm_model import LSTMPassDetectionModel

mlflow.set_experiment("Pass Detection")
mlflow.set_tracking_uri(uri="http://127.0.0.1:8080")


epochs = list(range(1, 10))

with mlflow.start_run(run_name="TEST"):
    for epoch in epochs:
        pytorch_model = LSTMPassDetectionModel(12)

        """LOGGING"""
        log_parameters = {
            'num_epochs': epoch,
        }

        log_metrics = {
            'brier_score': epoch * random(),
        }

        with mlflow.start_run(nested=True, run_name=str(epoch)):
            mlflow.log_params(log_parameters)
            mlflow.log_metrics(log_metrics)
            mlflow.set_tag("Training Info", "Basic pass detection model")
            # mlflow.pytorch.log_model(
            #     pytorch_model=pytorch_model,
            #     artifact_path="model",
            #     registered_model_name=model_name,
            # )
