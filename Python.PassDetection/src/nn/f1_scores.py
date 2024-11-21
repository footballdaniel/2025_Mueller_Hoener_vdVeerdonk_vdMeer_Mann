from typing import List, Tuple

from sklearn.metrics import precision_score, recall_score, f1_score

from src.domain import SampleWithFeatures


def predict_precision_recall_f1(samples: List[SampleWithFeatures]) -> Tuple[float, float, float]:
    all_labels = []
    all_predictions = []

    for sample in samples:
        if sample.pass_probability is None:
            continue  # Skip samples without a probability

        label = int(sample.is_a_pass)
        predicted = 1 if sample.pass_probability >= 0.5 else 0

        all_labels.append(label)
        all_predictions.append(predicted)

    precision = precision_score(all_labels, all_predictions, zero_division=0)
    recall = recall_score(all_labels, all_predictions, zero_division=0)
    f1 = f1_score(all_labels, all_predictions, zero_division=0)

    return precision, recall, f1
