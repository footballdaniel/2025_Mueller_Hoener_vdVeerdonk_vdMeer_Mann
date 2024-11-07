import numpy as np
import torch


def calculate_brier_score(labels, probabilities):
    """
    Calculates the Brier score for binary probabilistic predictions.

    Args:
        labels (torch.Tensor or numpy.ndarray): True binary labels (0 or 1).
        probabilities (torch.Tensor or numpy.ndarray): Predicted probabilities for the positive class.

    Returns:
        float: The Brier score.
    """
    if isinstance(labels, torch.Tensor):
        labels = labels.cpu().numpy()
    if isinstance(probabilities, torch.Tensor):
        probabilities = probabilities.cpu().numpy()

    brier_score = np.mean((probabilities - labels) ** 2)
    return brier_score
