from typing import List

from src.domain.run import Evaluation


def prediction_accuracy(evaluations: List[Evaluation]) -> float:
    correct = 0
    total = 0

    for evaluation in evaluations:
        if evaluation.predicted_label is None:
            continue  # Skip evaluations without a prediction

        label = int(evaluation.outcome_label)
        predicted = 1 if evaluation.predicted_label >= 0.5 else 0

        if predicted == label:
            correct += 1
        total += 1

    acc = correct / total if total > 0 else 0.0
    return acc
