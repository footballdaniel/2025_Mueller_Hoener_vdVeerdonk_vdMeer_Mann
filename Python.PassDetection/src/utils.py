import json
import subprocess
from dataclasses import is_dataclass, asdict
from enum import Enum

import mlflow
import psutil


class CustomEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, Enum):
            return o.name  # Serialize Enums as their name
        if is_dataclass(o):  # Support nested dataclasses
            return asdict(o)
        return super().default(o)
