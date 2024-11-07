import torch
from sklearn.metrics import precision_score, recall_score, f1_score

def calculate_classification_metrics(labels, predictions):
    """
    Calculates precision, recall, and F1 score for binary classification.

    Args:
        labels (torch.Tensor or numpy.ndarray): True binary labels (0 or 1).
        predictions (torch.Tensor or numpy.ndarray): Predicted binary labels (0 or 1).

    Returns:
        tuple: Precision, recall, and F1 score.
    """
    if isinstance(labels, torch.Tensor):
        labels = labels.cpu().numpy()
    if isinstance(predictions, torch.Tensor):
        predictions = predictions.cpu().numpy()

    precision = precision_score(labels, predictions)
    recall = recall_score(labels, predictions)
    f1 = f1_score(labels, predictions)
    return precision, recall, f1