from dataclasses import dataclass
from enum import Enum
from pathlib import Path
from typing import List

from src.infra.tiny_db.tiny_db_repository import TinyDbRepository


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
company_repository = TinyDbRepository(db_path, Company)

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
