from dataclasses import is_dataclass, fields
from enum import Enum


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
