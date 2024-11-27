from dataclasses import replace
from itertools import islice
from typing import Tuple, List

from sklearn.model_selection import train_test_split

from src.domain.inferences import Split
from src.infra.tiny_db.tiny_db_repository import ReadOnlyRepo


class TrainTestValidationSplitter:
    @staticmethod
    def split(
            repo: ReadOnlyRepo,
            number_samples: int,
            train_percentage: float,
            validation_percentage: float,
            test_percentage: float,
            random_state: int = 42
    ) -> Tuple[List[int], List[int], List[int]]:
        """
        Splits the dataset into stratified train, validation, and test indices,
        updates the repository with split information, and returns the indices.

        Args:
            repo (ReadOnlyRepo): The repository containing the samples.
            number_samples (int): Number of samples to fetch from the repository.
            train_percentage (float): Proportion of data for training.
            validation_percentage (float): Proportion of data for validation.
            test_percentage (float): Proportion of data for testing.
            random_state (int): Random seed for reproducibility.

        Returns:
            Tuple[List[int], List[int], List[int]]: Indices for train, validation, and test splits.
        """
        ids = []
        labels = []
        samples_iterator = islice(repo.get_all(), number_samples)

        samples = []
        for sample in samples_iterator:
            ids.append(sample.id)
            labels.append(int(sample.pass_event.is_a_pass))
            samples.append(sample)

        # Normalize percentages
        total = train_percentage + validation_percentage + test_percentage
        train_percentage /= total
        validation_percentage /= total
        test_percentage /= total

        # Create indices for stratified split
        indices = list(range(len(samples)))

        # Perform a single stratified split for train, validation, and test sets
        train_val_indices, test_indices, train_val_labels, test_labels = train_test_split(
            indices,
            labels,
            test_size=test_percentage,
            stratify=labels,
            random_state=random_state
        )

        # Further split train+validation into train and validation
        train_indices, val_indices, train_labels, val_labels = train_test_split(
            train_val_indices,
            train_val_labels,
            test_size=(validation_percentage / (train_percentage + validation_percentage)),
            stratify=train_val_labels,
            random_state=random_state
        )

        # Update repository with split information
        for idx in train_indices:
            sample = replace(samples[idx], inference=replace(samples[idx].inference, split=Split.TRAIN))
            repo.add(sample)
        for idx in val_indices:
            sample = replace(samples[idx], inference=replace(samples[idx].inference, split=Split.VALIDATION))
            repo.add(sample)
        for idx in test_indices:
            sample = replace(samples[idx], inference=replace(samples[idx].inference, split=Split.TEST))
            repo.add(sample)

        return train_indices, val_indices, test_indices
