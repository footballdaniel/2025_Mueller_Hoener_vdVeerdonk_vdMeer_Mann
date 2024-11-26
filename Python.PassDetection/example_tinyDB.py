from dataclasses import dataclass, is_dataclass, fields
from enum import Enum
from typing import List, Type, TypeVar, get_origin, get_args

from tinydb import TinyDB, Query


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


# Corrected custom serialization to handle Enums and ensure all fields are included
def custom_asdict(obj):
    if is_dataclass(obj):
        result = {}
        for field in fields(obj):
            key = field.name
            value = getattr(obj, key)
            if isinstance(value, Enum):
                result[key] = value.value  # Convert Enum to its value
            elif is_dataclass(value):
                result[key] = custom_asdict(value)  # Recursively serialize dataclass
            elif isinstance(value, list):
                result[key] = [custom_asdict(item) if is_dataclass(item) else item for item in value]
            else:
                result[key] = value
        return result
    return obj


# Initialize TinyDB
db = TinyDB('db.json')

# Clear the database for testing purposes
db.truncate()

# Create an example Company instance
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

# Serialize and store in TinyDB
serialized_company = custom_asdict(company)
db.insert(serialized_company)

# Helper functions for deserialization
T = TypeVar("T")


def from_dict(cls: Type[T], data: dict) -> T:
    field_types = {f: cls.__dataclass_fields__[f].type for f in cls.__dataclass_fields__}
    return cls(**{
        field: _deserialize_value(field_types[field], data[field])
        for field in field_types
    })


def _deserialize_value(field_type, value):
    # Handle lists
    if get_origin(field_type) is list:
        item_type = get_args(field_type)[0]
        return [_deserialize_value(item_type, item) for item in value]

    # Handle nested dataclasses
    if is_dataclass(field_type):
        return from_dict(field_type, value)

    # Handle enums
    if isinstance(field_type, type) and issubclass(field_type, Enum):
        return field_type(value)

    # For primitives or other types, return the value directly
    return value


# Query TinyDB and deserialize
CompanyQuery = Query()
result = db.search(CompanyQuery.id == 1)[0]
company_instance = from_dict(Company, result)

print(company_instance)
