from dataclasses import dataclass
from sys import maxsize


@dataclass
class Score:
    """Scores dataclass to store the scores of the model."""
    brier_score: float = maxsize  # Very high worst-case score
    accuracy: float = 0.0  # Worst possible accuracy
    f1_score: float = 0.0  # Worst possible F1 score
    precision: float = 0.0  # Worst possible precision
    recall: float = 0.0  # Worst possible recall
