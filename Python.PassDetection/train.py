import json
import pickle
import subprocess
from dataclasses import asdict, replace
from pathlib import Path
from typing import List

import mlflow
import onnx
import torch
import torch.nn as nn
from torch.utils.data import Subset, DataLoader

from src.domain.configurations import ConfigurationParser
from src.domain.inferences import Inference
from src.domain.run import Run, Scores, Evaluation
from src.features.feature_registry import FeatureRegistry
from src.infra.kpi.accuracy import prediction_accuracy
from src.infra.kpi.brier import prediction_brier
from src.infra.kpi.f1_scores import predict_precision_recall_f1
from src.infra.nn.model_registry import ModelRegistry
from src.infra.nn.pass_dataset import PassDataset
from src.infra.tiny_db.tiny_db_repository import RepositoryWithInMemoryCache
from src.services.feature_engineer import FeatureEngineer
from src.services.ingester import DataIngester
from src.services.label_creator import LabelCreator
from src.services.plotter import plot_sample_with_features
from src.services.recording_parser import RecordingParser
from src.services.traintestsplitter import TrainTestValidationSplitter
from src.utils import CustomEncoder

"""LOGGING"""
subprocess.Popen(
    f"mlflow server --host 127.0.0.1 --port 8080",
    shell=True,
    stdout=subprocess.DEVNULL,
    stderr=subprocess.DEVNULL,
    stdin=subprocess.DEVNULL,
    start_new_session=True
)
mlflow.set_tracking_uri(uri="http://127.0.0.1:8080")
mlflow.set_experiment("Pass Detection")

"""CONFIGURATIONS"""
architecture = "LSTM"
number_samples_to_analyze = 1_000_000
train_percentage = 0.7
validation_percentage = 0.2
test_percentage = 0.1
plot_dir = Path("plots")
output_dir_model = Path("../Unity.Interactions/Assets/Resources")
save_plots = True

"""PIPELINE"""
with mlflow.start_run(run_name=architecture):
    """CREATING SAMPLES"""
    recordings = DataIngester.ingest(Path("../Data/Pilot_4"))
    parser = RecordingParser()
    labeler = LabelCreator()
    repo = RepositoryWithInMemoryCache(recordings, parser, labeler)

    """SPLITTING DATASET"""
    train_indices, validation_indices, test_indices, num_samples = TrainTestValidationSplitter.split(
        repo,
        number_samples_to_analyze,
        train_percentage,
        validation_percentage,
        test_percentage,
    )

    runs: List[Run] = []
    config_matrix = ConfigurationParser.generate_configurations(Path("config.yaml"))
    for run_idx, config in enumerate(config_matrix):
        """FEATURE ENGINEERING"""
        engineer = FeatureEngineer()

        for feature in config.features:
            feature_instance = FeatureRegistry.create(feature)
            engineer.add_feature(feature_instance)

        dataset = PassDataset(repo, num_samples, engineer)

        train_dataset = Subset(dataset, train_indices)
        validation_dataset = Subset(dataset, validation_indices)
        test_dataset = Subset(dataset, test_indices)

        training_loader = DataLoader(train_dataset, batch_size=config.batch_size, shuffle=True)
        validation_loader = DataLoader(validation_dataset, batch_size=config.batch_size, shuffle=False)
        testing_loader = DataLoader(test_dataset, batch_size=config.batch_size, shuffle=False)

        """MODEL"""
        input_size = engineer.input_size
        learning_rate = config.learning_rate
        num_epochs = config.epochs
        patience = config.early_stopping_patience

        model = ModelRegistry.create(config.model_type, input_size)
        optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)

        if config.loss_function == "nn.BCELoss":
            criterion = nn.BCELoss()
        else:
            raise ValueError("Invalid loss function")

        """EARLY STOPPING"""
        best_val_loss = float('inf')
        epochs_no_improve = 0
        early_stop = False

        """TRAINING"""
        device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
        model.to(device)

        epoch_index = 0
        for epoch in range(num_epochs):
            epoch_index = epoch
            if early_stop:
                break

            model.train()
            total_loss = 0

            for inputs, labels in training_loader:
                inputs = inputs.to(device)
                labels = labels.to(device).view(-1, 1)
                optimizer.zero_grad()
                outputs = model(inputs)
                loss = criterion(outputs, labels)
                loss.backward()
                optimizer.step()
                total_loss += loss.item() * inputs.size(0)

            avg_loss = total_loss / len(train_indices)
            print(f"Epoch [{epoch + 1}/{num_epochs}], Loss: {avg_loss:.4f}")

            # Validation
            model.eval()
            total_val_loss = 0
            correct = 0
            with torch.no_grad():
                for inputs, labels in validation_loader:
                    inputs = inputs.to(device)
                    labels = labels.to(device).view(-1, 1)
                    outputs = model(inputs)
                    loss = criterion(outputs, labels)
                    total_val_loss += loss.item() * inputs.size(0)
                    predicted = (outputs >= 0.5).float()
                    correct += (predicted == labels).sum().item()

            avg_val_loss = total_val_loss / len(validation_indices)
            accuracy = correct / len(validation_indices)
            print(f"Validation Loss: {avg_val_loss:.4f}, Accuracy: {accuracy:.4f}")

            # Check for improvement
            if avg_val_loss < best_val_loss:
                best_val_loss = avg_val_loss
                epochs_no_improve = 0
            else:
                epochs_no_improve += 1
                if epochs_no_improve >= patience:
                    print(f'Early stopping at epoch {epoch + 1}')
                    early_stop = True

        """EVALUATION ON VALIDATION SET"""
        model.eval()
        evaluations: List[Evaluation] = []
        with torch.no_grad():
            for idx in validation_indices:
                sample = repo.get(idx)
                input_tensor, label = dataset[idx]
                input_tensor = input_tensor.unsqueeze(0).to(device)
                output = model(input_tensor)
                probability = round(output.item(), 3)
                evaluations.append(Evaluation(sample.inference.label, probability))

        brier_score = prediction_brier(evaluations)
        accuracy = prediction_accuracy(evaluations)
        precision, recall, f1_score = predict_precision_recall_f1(evaluations)

        """LOGGING"""
        log_parameters = asdict(config)

        run = Run(
            model=model,
            config=config,
            score=brier_score,
        )

        test_scores = Scores(
            brier_score=brier_score,
            f1_score=f1_score,
            precision=precision,
            recall=recall,
            accuracy=accuracy,
        )

        runs.append(run)

        with mlflow.start_run(nested=True, run_name=str(run_idx)):
            mlflow.log_params(asdict(config))
            mlflow.log_metrics(asdict(test_scores))

    """EVALUATION BEST MODEL ON TEST SET"""
    best_run = min(runs, key=lambda r: r.score)
    model = best_run.model

    evaluations: List[Evaluation] = []
    with torch.no_grad():
        for idx in test_indices:
            sample = repo.get(idx)
            input_tensor, label = dataset[idx]
            input_tensor = input_tensor.unsqueeze(0).to(device)
            output = model(input_tensor)
            probability = output.item()
            evaluations.append(Evaluation(sample.inference.label, probability))

    brier_score = prediction_brier(evaluations)
    accuracy = prediction_accuracy(evaluations)
    precision, recall, f1_score = predict_precision_recall_f1(evaluations)

    test_scores = Scores(
        brier_score=brier_score,
        f1_score=f1_score,
        precision=precision,
        recall=recall,
        accuracy=accuracy,
    )

    # """SAVE PREDICTIONS TO DATASET"""
    # model = best_run.model
    # engineer = FeatureEngineer()
    # for feature in best_run.config.features:
    #     feature_instance = FeatureRegistry.create(feature)
    #     engineer.add_feature(feature_instance)

    # dataset = PassDataset(repo, num_samples, engineer)
    #
    # with torch.no_grad():
    #     for idx in range(num_samples):
    #         input_tensor, label = dataset[idx]
    #         input_tensor = input_tensor.unsqueeze(0).to(device)
    #         output = model(input_tensor)
    #         probability = output.item()
    #         sample = repo.get(idx)
    #         computed_features = engineer.engineer(sample.recording.input_data)
    #         sample_with_prediction = replace(
    #             sample,
    #             inference=Inference(
    #                 prediction=probability,
    #                 split=sample.inference.split,
    #                 computed_features=computed_features,
    #                 label=sample.recording.contains_a_pass,
    #             )
    #         )
    #         repo.add(sample_with_prediction)

    # """SAVE DATASET"""
    # samples = [repo.get(idx) for idx in test_indices]
    # # Save some to folder
    # with open('dataset.pkl', 'wb') as f:
    #     pickle.dump(samples, f)
    #
    # with open("dataset.json", 'w') as f:
    #     json.dump(samples, f, cls=CustomEncoder)
    #
    # """PLOT SAMPLES"""
    # for idx, id in enumerate(samples):
    #     if not save_plots:
    #         break
    #     plot_sample_with_features(id, plot_dir)

    """EXPORT ONNX"""
    # Path for ONNX file
    onnx_file_paths = ["pass_detection_model.onnx", output_dir_model.joinpath("pass_detection_model.onnx")]
    model.to('cpu')
    model.eval()

    # Grab only the first sample from the last batch of val_loader for export
    example_input = inputs[0:1].cpu()  # First sample of last batch with batch size 1
    with torch.no_grad():
        example_output = model(example_input)
        example_output_values = example_output.squeeze().tolist()

        print(f"Example input shape: {example_input.shape}")
        print(f"Example output shape: {example_output.shape}")

    for onnx_file_path in onnx_file_paths:
        torch.onnx.export(
            model,
            example_input,
            onnx_file_path,
            export_params=True,
            opset_version=15,
            do_constant_folding=True,
            input_names=['input'],
            output_names=['output'],
        )

        onnx_model = onnx.load(onnx_file_path)

        onnx_model.doc_string = "LSTM Model for pass detection"
        onnx_model.domain = "com.tactivesport.feedback"
        onnx_model.producer_name = "Tactive Sport"
        onnx_model.producer_version = "1"
        onnx_model.model_version = 1

        feature_names = [feature for feature in config.features]
        metadata_props = onnx_model.metadata_props.add()
        metadata_props.key = "features"
        metadata_props.value = json.dumps(feature_names)  # Serialize the list of feature names as JSON

        metadata_props = onnx_model.metadata_props.add()
        metadata_props.key = "example_input"
        metadata_props.value = str(example_input.tolist())  # Store example input as string

        metadata_props = onnx_model.metadata_props.add()
        metadata_props.key = "example_output"
        metadata_props.value = str(example_output_values)  # Store example output as string
        onnx.save(onnx_model, onnx_file_path)

    mlflow.log_params(asdict(config))
    mlflow.log_metrics(asdict(test_scores))
    mlflow.log_artifact(onnx_file_paths[0])
