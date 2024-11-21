from typing import List

from src.domain.samples import Sample


def prediction_brier(samples: List[Sample]) -> float:
    total_loss = 0.0
    total = 0

    for sample in samples:
        if sample.inference.pass_probability is None:
            continue  # Skip samples without a probability

        label = float(sample.pass_info.is_a_pass)
        probability = sample.inference.pass_probability

        loss = (probability - label) ** 2
        total_loss += loss
        total += 1

    score = total_loss / total if total > 0 else 0.0
    return score
