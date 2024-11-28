from pathlib import Path
from typing import TypeVar, List, Iterator

from tinydb import Query, TinyDB

from src.domain.repositories import BaseRepository
from src.domain.samples import IngestableRecording
from src.infra.tiny_db.deserializer import custom_from_dict
from src.infra.tiny_db.serializer import custom_asdict
from src.services.label_creator import LabelCreator
from src.services.recording_parser import RecordingParser

T = TypeVar('T')


class RepositoryWithInMemoryCache(BaseRepository[T]):
    def __init__(
            self,
            recordings: List[IngestableRecording],
            parser: RecordingParser,
            label_creator: LabelCreator,
    ):
        self.recordings = recordings
        self.parser = parser
        self.label_creator = label_creator
        self._samples = {}  # In-memory storage: id -> sample
        self._all_samples_loaded = False  # Flag to check if all samples are loaded

    def get(self, id: int) -> T:
        # Check if the sample is already in the in-memory storage
        if id in self._samples:
            return self._samples[id]

        # If not, generate samples until we find it
        for sample in self._generate_samples():
            if sample.id == id:
                self._samples[id] = sample  # Cache the sample
                return sample

        raise ValueError(f"Entity with id {id} not found")

    def get_all(self) -> Iterator[T]:
        # If all samples are already loaded, return them from the cache
        if self._all_samples_loaded:
            return iter(self._samples.values())

        # Otherwise, generate all samples and cache them
        for sample in self._generate_samples():
            self._samples[sample.id] = sample

        self._all_samples_loaded = True  # Set the flag after loading all samples
        return iter(self._samples.values())

    def _generate_samples(self) -> Iterator[T]:
        current_id = 0
        for recording in self.recordings:
            self.parser.read_recording_from_json(recording.timeseries_file)
            self.parser.read_pass_events_from_csv(recording.event_file)
            samples = self.label_creator.generate(
                self.parser.recording, start_id=current_id
            )
            for sample in samples:
                yield sample
                current_id += 1  # Increment ID for each sample

    def add(self, sample: T):
        # Add the new sample to the in-memory storage
        self._samples[sample.id] = sample
        a = 1


class RepositoryWithCache(RepositoryWithInMemoryCache):
    def __init__(self, recordings: List[IngestableRecording], db_path: Path, parser: RecordingParser, label_creator: LabelCreator):
        super().__init__(recordings, parser, label_creator)
        self.db = TinyDB(db_path)

    def get(self, id: int) -> T:
        result = self.db.search(Query()['id'] == id)
        if result:
            return custom_from_dict(self.entity_cls, result[0])
        return super().get(id)

    def get_all(self) -> Iterator[T]:
        cached_ids = {item['id'] for item in self.db.all()}

        for item in self.db.all():
            yield custom_from_dict(self.entity_cls, item)

        current_id = max(cached_ids, default=-1) + 1
        for sample in super()._generate_samples():
            if sample.id >= current_id and sample.id not in cached_ids:
                self.db.insert(custom_asdict(sample))
                yield sample

    def add(self, sample):
        self.db.insert(custom_asdict(sample))