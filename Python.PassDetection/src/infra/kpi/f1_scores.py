from typing import List, Tuple

from sklearn.metrics import precision_score, recall_score, f1_score

from src.domain.run import Evaluation


def predict_precision_recall_f1(evaluations: List[Evaluation]) -> Tuple[float, float, float]:
    all_labels = []
    all_predictions = []

    for evaluation in evaluations:
        if evaluation.predicted_label is None:
            continue  # Skip evaluations without a prediction

        label = int(evaluation.outcome_label)
        predicted = 1 if evaluation.predicted_label >= 0.5 else 0

        all_labels.append(label)
        all_predictions.append(predicted)

    precision = precision_score(all_labels, all_predictions, zero_division=0)
    recall = recall_score(all_labels, all_predictions, zero_division=0)
    f1 = f1_score(all_labels, all_predictions, zero_division=0)

    return precision, recall, f1
