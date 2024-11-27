from abc import ABC, abstractmethod
from typing import Generic, TypeVar, Iterable

T = TypeVar("T")  # Unbounded TypeVar for generic entity


class Repository(ABC, Generic[T]):

    @abstractmethod
    def get_all(self) -> Iterable[T]:
        ...

    @abstractmethod
    def get(self, id: int) -> T:
        ...

    @abstractmethod
    def add(self, entity: T) -> None:
        ...
