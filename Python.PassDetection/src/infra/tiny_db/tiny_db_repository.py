from typing import TypeVar, Iterator, Type
from pathlib import Path

from tinydb import Query, TinyDB

from src.domain.repositories import BaseRepository
from src.domain.samples import Sample
from src.infra.tiny_db.deserializer import custom_from_dict
from src.infra.tiny_db.serializer import custom_asdict


class RepositoryWithInMemoryCache(BaseRepository):
    def __init__(
            self,
            samples: Iterator[Sample],
    ):
        self._samples = {}
        self._all_samples_loaded = False  # Flag to check if all samples are loaded
        self.samples_iterator = samples

    def get(self, id: int) -> Sample:
        # Check if the sample is already in the in-memory storage
        if id in self._samples:
            return self._samples[id]

        # Load samples from the iterator until we find the desired sample
        for sample in self._load_samples_until(id):
            if sample.id == id:
                return sample

        raise ValueError(f"Entity with id {id} not found")

    def get_all(self) -> Iterator[Sample]:
        # If all samples are already loaded, return them from the cache
        if self._all_samples_loaded:
            return iter(self._samples.values())

        # Otherwise, load all samples from the iterator
        for sample in self.samples_iterator:
            self._samples[sample.id] = sample

        self._all_samples_loaded = True  # Set the flag after loading all samples
        return iter(self._samples.values())

    def _load_samples_until(self, target_id: int) -> Iterator[Sample]:
        for sample in self.samples_iterator:
            self._samples[sample.id] = sample
            if sample.id == target_id:
                yield sample
                break
            yield sample

    def add(self, sample: Sample):
        # Add the new sample to the in-memory storage
        self._samples[sample.id] = sample


class RepositoryWithCache(RepositoryWithInMemoryCache):
    def __init__(
            self,
            samples: Iterator[Sample],
            db_path: Path,
            entity_cls: Type[Sample],
    ):
        super().__init__(samples, entity_cls)
        self.db = TinyDB(db_path)

    def get(self, id: int) -> Sample:
        result = self.db.search(Query()['id'] == id)
        if result:
            return custom_from_dict(self.entity_cls, result[0])

        # Load samples until we find the desired one
        for sample in self._load_samples_until(id):
            self.db.insert(custom_asdict(sample))
            if sample.id == id:
                return sample

        raise ValueError(f"Entity with id {id} not found")

    def get_all(self) -> Iterator[Sample]:
        cached_ids = {item['id'] for item in self.db.all()}

        # Yield cached samples first
        for item in self.db.all():
            sample = custom_from_dict(self.entity_cls, item)
            self._samples[sample.id] = sample
            yield sample

        # Then load and yield new samples from the iterator
        for sample in self.samples_iterator:
            if sample.id not in cached_ids:
                self.db.insert(custom_asdict(sample))
                self._samples[sample.id] = sample
                yield sample

        self._all_samples_loaded = True  # Set the flag after loading all samples

    def add(self, sample: Sample):
        self.db.insert(custom_asdict(sample))
        super().add(sample)
