from dataclasses import dataclass

from src.domain.configurations import Configuration
from src.infra.nn.base_nn_model import BaseModel


@dataclass
class Run:
    model: BaseModel
    config: Configuration
    score: float


@dataclass
class Scores:
    brier_score: float
    f1_score: float
    precision: float
    recall: float
    accuracy: float


@dataclass
class Evaluation:
    outcome_label: int
    predicted_label: int
