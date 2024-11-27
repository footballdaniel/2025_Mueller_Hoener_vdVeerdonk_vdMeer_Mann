from dataclasses import dataclass
from enum import Enum
from pathlib import Path
from typing import List, Iterable, Type, TypeVar

from tinydb import TinyDB, Query

from src.domain.repositories import BaseRepository
from src.infra.tiny_db.deserializer import custom_from_dict
from src.infra.tiny_db.serializer import custom_asdict

T = TypeVar('T')


class TinyDbRepositoryExample():

    def get_all(self) -> Iterable[T]:
        for item in self.db.all():
            yield custom_from_dict(self.entity_cls, item)

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


# Define an Enum for EmployeeType
class EmployeeType(Enum):
    FULL_TIME = "full_time"
    PART_TIME = "part_time"
    INTERN = "intern"


@dataclass
class Address:
    street: str
    city: str


@dataclass
class Employee:
    name: str
    age: int
    address: Address
    employee_type: EmployeeType


@dataclass
class Company:
    id: int
    name: str
    employees: List[Employee]


db_path = Path('db.json')
company_repository = TinyDbRepositoryExample(db_path, Company)

# Clear the database
company_repository.db.truncate()

# Add a company
company = Company(
    id=1,
    name="TechCorp",
    employees=[
        Employee(
            name="Alice", age=30, address=Address(street="123 Main St", city="Metropolis"),
            employee_type=EmployeeType.FULL_TIME
        ),
        Employee(
            name="Bob", age=40, address=Address(street="456 Elm St", city="Gotham"),
            employee_type=EmployeeType.PART_TIME
        ),
    ],
)
company_repository.add(company)
retrieved_company = company_repository.get(1)

print(retrieved_company)
