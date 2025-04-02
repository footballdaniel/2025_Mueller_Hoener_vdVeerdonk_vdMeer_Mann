from dataclasses import dataclass
from pathlib import Path
from typing import Optional

from src.domain import TrialCollection
from src.persistence import Persistence


@dataclass
class RegressionModel:
    model_path: Path
    persistence: Persistence
    trials: TrialCollection


def regression_1(trials: TrialCollection, model_path: Path, persistence: Persistence) -> None:
    model = RegressionModel(
        model_path=model_path,
        persistence=persistence,
        trials=trials
    ) 