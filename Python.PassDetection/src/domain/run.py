from dataclasses import dataclass

from src.domain.configurations import Configuration
from src.infra.nn import BaseModel


@dataclass
class Run:
    model: BaseModel
    config: Configuration


@dataclass
class Scores:
    brier_score: float
    f1_score: float
    precision: float
    recall: float
    accuracy: float
