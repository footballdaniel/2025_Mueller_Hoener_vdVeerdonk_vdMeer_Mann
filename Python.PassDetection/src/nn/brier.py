from typing import List

from src.domain import SampleWithFeatures


def prediction_brier(samples: List[SampleWithFeatures]) -> float:
    total_loss = 0.0
    total = 0

    for sample in samples:
        if sample.pass_probability is None:
            continue  # Skip samples without a probability

        label = float(sample.is_a_pass)
        probability = sample.pass_probability

        loss = (probability - label) ** 2
        total_loss += loss
        total += 1

    score = total_loss / total if total > 0 else 0.0
    return score
