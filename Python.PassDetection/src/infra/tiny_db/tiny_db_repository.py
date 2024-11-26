from pathlib import Path
from typing import TypeVar, Type

from tinydb import Query, TinyDB

from src.domain.repositories import Repository
from src.infra.tiny_db.deserializer import custom_from_dict
from src.infra.tiny_db.serializer import custom_asdict

T = TypeVar('T')


class TinyDbRepository(Repository[T]):

    def __init__(self, path: Path, entity_cls: Type[T]):
        self.db = TinyDB(path)
        self.entity_cls = entity_cls

    def get(self, id: int) -> T:
        result = self.db.search(Query()['id'] == id)
        if not result:
            raise ValueError(f"Entity with ID {id} not found")
        return custom_from_dict(self.entity_cls, result[0])

    def add(self, entity: T) -> None:
        serialized = custom_asdict(entity)
        self.db.insert(serialized)

    def remove(self, entity: T) -> None:
        self.db.remove(Query()['id'] == entity.id)
