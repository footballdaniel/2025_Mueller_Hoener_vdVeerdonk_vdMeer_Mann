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


def start_local_mlflow_server():
    for proc in psutil.process_iter(['pid', 'name']):
        if 'mlflow' in proc.info['name']:
            return

    subprocess.Popen("mlflow server --host 127.0.0.1 --port 8080", shell=True)
    mlflow.set_tracking_uri(uri="http://127.0.0.1:8080")
