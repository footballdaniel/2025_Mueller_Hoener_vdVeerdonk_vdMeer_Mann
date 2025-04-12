from pathlib import Path
from typing import List

import numpy as np
import matplotlib.pyplot as plt
from sklearn.cluster import KMeans

from src.domain import Trial
from src.persistence import Persistence
from src.services import DistanceCalculator, TimeCalculator, MovementCalculator


def plot_elbow_method(trials: List[Trial], max_clusters: int, persistence: Persistence) -> None:
    """
    Plot the elbow method to determine the optimal number of clusters.
    """
    wcss = []
    trial_features = np.array([
        [
            DistanceCalculator.distance_between_last_touch_and_pass(trial),
            TimeCalculator.time_between_last_change_of_direction_and_pass(trial),
            MovementCalculator.number_lateral_changes_of_direction(trial)
        ]
        for trial in trials
    ])

    # Remove NaN values
    trial_features = trial_features[~np.isnan(trial_features).any(axis=1)]

    for i in range(1, max_clusters + 1):
        kmeans = KMeans(n_clusters=i, init='k-means++', random_state=42)
        kmeans.fit(trial_features)
        wcss.append(kmeans.inertia_)

    plt.figure(figsize=(10, 6))
    plt.plot(range(1, max_clusters + 1), wcss)
    plt.title('Elbow Method')
    plt.xlabel('Number of clusters')
    plt.ylabel('WCSS')
    plt.grid(True)

    persistence.save_figure(plt.gcf(), Path("elbow_method.png"))
    plt.close() 