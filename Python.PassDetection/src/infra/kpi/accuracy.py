from typing import List

from src.domain.samples import Sample


def prediction_accuracy(samples: List[Sample]) -> float:
    correct = 0
    total = 0

    for sample in samples:
        if sample.inference.pass_probability is None:
            continue  # Skip samples without a probability

        label = int(sample.pass_event.is_a_pass)
        predicted = 1 if sample.inference.pass_probability >= 0.5 else 0

        if predicted == label:
            correct += 1
        total += 1

    acc = correct / total if total > 0 else 0.0
    return acc
