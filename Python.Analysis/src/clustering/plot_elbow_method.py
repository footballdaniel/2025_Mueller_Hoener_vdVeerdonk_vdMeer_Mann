from pathlib import Path
from typing import List

import numpy as np
from sklearn.cluster import KMeans

from src.domain import Trial
from src.persistence import Persistence
from src.services import DistanceCalculator, TimeCalculator, MovementCalculator


def analyze_number_clusters(trials: List[Trial], max_clusters: int, persistence: Persistence, path: Path) -> None:
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

    # Calculate total sum of squares
    total_ss = np.sum((trial_features - np.mean(trial_features, axis=0)) ** 2)
    
    # Calculate WCSS and explained variability for each number of clusters
    results = []
    for i in range(1, max_clusters + 1):
        kmeans = KMeans(n_clusters=i, init='k-means++', random_state=42)
        kmeans.fit(trial_features)
        wcss.append(kmeans.inertia_)
        
        # Calculate explained variability
        explained_var = 1 - (kmeans.inertia_ / total_ss)
        results.append(f"Clusters: {i}, WCSS: {kmeans.inertia_:.2f}, Explained Variability: {explained_var:.2%}")

    # Save results to text file
    persistence.save_model("\n".join(results), path)