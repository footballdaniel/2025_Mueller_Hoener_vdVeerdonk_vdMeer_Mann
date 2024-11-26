from dataclasses import is_dataclass
from enum import Enum
from typing import Type, TypeVar, get_origin, get_args

T = TypeVar("T")


def custom_from_dict(cls: Type[T], data: dict) -> T:
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
        return custom_from_dict(field_type, value)

    # Handle enums
    if isinstance(field_type, type) and issubclass(field_type, Enum):
        return field_type(value)

    # For primitives or other types, return the value directly
    return value
