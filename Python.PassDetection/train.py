import glob
import os
from pathlib import Path

import mlflow
import onnx
import torch
import torch.nn as nn
from torch.utils.data import Subset, DataLoader

from src.domain.configurations import ConfigurationParser
from src.features.feature_registry import FeatureRegistry
from src.domain.inferences import Split
from src.nn.model_registry import ModelRegistry
from src.kpi.accuracy import prediction_accuracy
from src.kpi.brier import prediction_brier
from src.kpi.f1_scores import predict_precision_recall_f1
from src.nn.pass_dataset import PassDataset
from src.services.augmenter import Augmenter
from src.services.feature_engineer import FeatureEngineer
from src.services.label_creator import LabelCreator
from src.services.recording_parser import RecordingParser

"""LOGGING"""
mlflow.set_experiment("Pass Detection")
mlflow.set_tracking_uri(uri="http://127.0.0.1:8080")

"""READING CONFIGURATIONS"""
architecture = "LSTM"
config_matrix = ConfigurationParser.generate_configurations(Path("config.yaml"))

with mlflow.start_run(run_name=architecture):
    """READING RECORDINGS"""
    pattern = "../Data/Pilot_4/**/*.csv"
    plot_dir = Path("plots")
    output_dir_model = Path("../Unity.Interactions/Assets/Resources")
    save_plots = False

    recordings = []
    for filename in glob.iglob(pattern, recursive=True):
        print(f"Processing file: {filename}")
        json_filename = filename.replace(".csv", ".json")
        if not os.path.isfile(json_filename):
            print("No JSON file found for", filename)
            continue

        parser = RecordingParser()
        parser.read_recording_from_json(json_filename)
        parser.read_pass_events_from_csv(filename)
        recordings.append(parser.recording)

    """SAMPLING AND AUGMENTATION"""
    labeled_samples = LabelCreator.generate(recordings)
    augmented_samples = Augmenter.augment(labeled_samples, only_augment_passes=True)

    for run_idx, config in enumerate(config_matrix):

        """FEATURE ENGINEERING"""
        engineer = FeatureEngineer()

        for feature in config.features:
            feature_instance = FeatureRegistry.create(feature)
            engineer.add_feature(feature_instance)

        calculated_features = engineer.engineer(augmented_samples)

        # SPLIT PYTORCH DATASET
        dataset = PassDataset(calculated_features)
        torch.manual_seed(42)  # Set the seed
        train_size = int(0.8 * len(dataset))
        validation_size = len(dataset) - train_size
        train_indices, val_indices = torch.utils.data.random_split(range(len(dataset)), [train_size, validation_size])

        for i in train_indices:
            dataset.samples[i].inference.split = Split.TRAIN
        for i in val_indices:
            dataset.samples[i].inference.split = Split.VALIDATION

        # Create Subset datasets for training and validation
        train_dataset = Subset(dataset, train_indices)
        validation_dataset = Subset(dataset, val_indices)

        positive_training = sum(
            1 for sample in dataset.samples if sample.inference.split == Split.TRAIN and sample.pass_event.is_a_pass)
        negative_training = sum(
            1 for sample in dataset.samples if sample.inference.split == Split.TRAIN and not sample.pass_event.is_a_pass)
        positive_validation = sum(
            1 for sample in dataset.samples if sample.inference.split == Split.VALIDATION and sample.pass_event.is_a_pass)
        negative_validation = sum(
            1 for sample in dataset.samples if
            sample.inference.split == Split.VALIDATION and not sample.pass_event.is_a_pass)
        print(f"Training set: Size:{train_size} Positive:{positive_training} Negative:{negative_training}")
        print(f"Validation set: Size:{validation_size} Positive:{positive_validation} Negative:{negative_validation}")

        training_loader = DataLoader(train_dataset, batch_size=config.batch_size, shuffle=True)
        validation_loader = DataLoader(validation_dataset, batch_size=config.batch_size, shuffle=False)

        """MODEL"""
        input_size = engineer.input_size
        learning_rate = config.learning_rate
        num_epochs = config.epochs
        patience = config.early_stopping_patience

        model = ModelRegistry.create(config.model_type, input_size)

        if config.loss_function == "nn.BCELoss":
            criterion = nn.BCELoss()
        else:
            raise ValueError("Invalid loss function")

        optimizer = torch.optim.Adam(model.parameters(), lr=learning_rate)

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

            avg_loss = total_loss / len(training_loader.dataset)
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

            avg_val_loss = total_val_loss / len(validation_loader.dataset)
            accuracy = correct / len(validation_loader.dataset)
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

        """SAVE MODEL"""
        checkpoint = {
            'model_state_dict': model.state_dict(),
            'optimizer_state_dict': optimizer.state_dict(),
            'input_size': input_size,
            'epoch': epoch_index,
        }
        torch.save(checkpoint, 'pass_detection_model.pth')

        """EVALUATION"""
        model.eval()
        with torch.no_grad():
            for idx in range(len(dataset)):
                sample = dataset.samples[idx]
                input_tensor, label = dataset[idx]
                input_tensor = input_tensor.unsqueeze(0).to(device)
                output = model(input_tensor)
                probability = output.item()
                sample.inference.pass_probability = round(probability, 3)

        brier_score = prediction_brier(dataset.samples)
        accuracy = prediction_accuracy(dataset.samples)
        precision, recall, f1_score = predict_precision_recall_f1(dataset.samples)

        print(f"Brier Score: {brier_score:.4f}")
        print(f"Accuracy: {accuracy:.4f}")
        print(f"Precision: {precision:.4f}, Recall: {recall:.4f}, F1 Score: {f1_score:.4f}")

        # """SAVE DATASET"""
        # # Save some to folder
        # with open('dataset.pkl', 'wb') as f:
        #     pickle.dump(dataset.samples, f)
        #
        # with open("dataset.json", 'w') as f:
        #     json.dump([asdict(sample) for sample in dataset.samples], f, cls=CustomEncoder, indent=4)
        #
        # """PLOT SAMPLES"""
        # for idx, sample in enumerate(dataset.samples):
        #     if not save_plots:
        #         break
        #
        #     filename = f"sample_{sample.recording.trial_number}_{idx}_pass{sample.pass_event.is_a_pass}.png"
        #     fig = plot_sample_with_features(sample)
        #     plot_path = os.path.join(str(plot_dir), filename)
        #     fig.savefig(plot_path)
        #     plt.close(fig)

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

        """LOGGING"""
        log_parameters = {
            'num_epochs': config.epochs,
        }

        log_metrics = {
            'brier_score': brier_score,
            'accuracy': accuracy,
            'precision': precision,
            'recall': recall,
            'f1_score': f1_score
        }

        with mlflow.start_run(nested=True, run_name=str(run_idx)):
            mlflow.log_params(log_parameters)
            mlflow.log_metrics(log_metrics)
            mlflow.set_tag("Training Info", "Feedback Algorithm")

            # model_info = mlflow.pytorch.log_model(
            #     pytorch_model=model,
            #     artifact_path="model",
            #     registered_model_name=model_name,
            # )
