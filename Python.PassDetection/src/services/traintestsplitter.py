from itertools import islice
from typing import List

from sklearn.model_selection import train_test_split

from src.infra.tiny_db.tiny_db_repository import RepositoryWithInMemoryCache


class TrainTestValidationSplitter:

    def __init__(
            self,
            repo: RepositoryWithInMemoryCache,
            number_samples: int,
            train_percentage: float,
            validation_percentage: float,
            test_percentage: float,
            random_state: int = 42):
        ids = []
        labels = []
        samples_iterator = islice(repo.get_all(), number_samples)

        samples = []
        for sample in samples_iterator:
            ids.append(sample.id)
            labels.append(int(sample.contains_a_pass()))
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

        self._train_indices = train_indices
        self._validation_indices = val_indices
        self._test_indices = test_indices

    @property
    def validation_indices(self) -> List[int]:
        return self._validation_indices

    @property
    def test_indices(self) -> List[int]:
        return self._test_indices

    @property
    def train_indices(self) -> List[int]:
        return self._train_indices

    @property
    def number_samples(self) -> int:
        return len(self._train_indices) + len(self._validation_indices) + len(self._test_indices)
