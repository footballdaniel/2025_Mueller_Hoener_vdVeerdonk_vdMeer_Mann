from typing import List

from sklearn.metrics import brier_score_loss

from src.domain.run import Evaluation


def prediction_brier(evaluations: List[Evaluation]) -> float:
    y_true = [evaluation.outcome_label for evaluation in evaluations]
    y_prob = [evaluation.predicted_label for evaluation in evaluations]

    return brier_score_loss(y_true, y_prob)