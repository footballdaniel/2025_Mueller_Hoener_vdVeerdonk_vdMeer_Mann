from typing import List

from src.domain import SampleWithFeatures


def prediction_accuracy(samples: List[SampleWithFeatures]) -> float:
    correct = 0
    total = 0

    for sample in samples:
        if sample.pass_probability is None:
            continue  # Skip samples without a probability

        label = int(sample.is_a_pass)
        predicted = 1 if sample.pass_probability >= 0.5 else 0

        if predicted == label:
            correct += 1
        total += 1

    acc = correct / total if total > 0 else 0.0
    return acc
