import json
import subprocess
import time
from dataclasses import is_dataclass, asdict
from enum import Enum

import requests


class CustomEncoder(json.JSONEncoder):
    def default(self, o):
        if isinstance(o, Enum):
            return o.name  # Serialize Enums as their name
        if is_dataclass(o):  # Support nested dataclasses
            return asdict(o)
        return super().default(o)


def manage_mlflow_server(host: str = '127.0.0.1', port: int = 8080):
    def is_server_running():
        try:
            response = requests.get(f"http://{host}:{port}")
            return response.status_code == 200
        except requests.exceptions.ConnectionError:
            return False

    if not is_server_running():
        # Start a detached MLflow server process
        subprocess.Popen(
            f"mlflow server --host {host} --port {port}",
            shell=True,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            stdin=subprocess.DEVNULL,
            start_new_session=True
        )



