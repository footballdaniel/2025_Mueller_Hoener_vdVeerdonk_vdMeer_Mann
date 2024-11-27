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


class Repository(BaseRepository[T]):
    def __init__(self, recordings: List[IngestableRecording], parser: RecordingParser, label_creator: LabelCreator):
        self.recordings = recordings
        self.parser = parser
        self.label_creator = label_creator

    def get(self, id: int) -> T:
        for sample in self._generate_samples():
            if sample.id == id:  # Assuming each sample has a unique `id` attribute
                return sample
        raise ValueError(f"Entity with id {id} not found")

    def get_all(self) -> Iterator[T]:
        return self._generate_samples()

    def _generate_samples(self) -> Iterator[T]:
        current_id = 0
        for recording in self.recordings:
            self.parser.read_recording_from_json(recording.timeseries_file)
            self.parser.read_pass_events_from_csv(recording.event_file)
            yield from self.label_creator.generate(self.parser.recording, start_id=current_id)
            current_id += len(self.parser.recording.input_data.user_dominant_foot_positions)

    def add(self, sample):
        pass


class RepositoryWithCache(Repository):
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