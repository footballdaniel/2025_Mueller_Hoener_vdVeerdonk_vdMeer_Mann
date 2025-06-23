from pathlib import Path
from typing import List

import numpy as np
from sklearn.cluster import KMeans

from src.domain import Trial
from src.persistence import Persistence
from src.services import DistanceCalculator, TimeCalculator, MovementCalculator


def analyze_number_clusters(trials: List[Trial], max_clusters: int, persistence: Persistence, path: Path) -> None:
    wcss = []
    trial_features = np.array([
        [
            DistanceCalculator.distance_between_last_touch_and_pass(trial),
            TimeCalculator.time_between_last_change_of_direction_and_pass(trial),
            MovementCalculator.number_lateral_changes_of_direction(trial)
        ]
        for trial in trials
    ])

    trial_features = trial_features[~np.isnan(trial_features).any(axis=1)]
    total_ss = np.sum((trial_features - np.mean(trial_features, axis=0)) ** 2)
    
    results = []
    for i in range(1, max_clusters + 1):
        kmeans = KMeans(n_clusters=i, init='k-means++', random_state=42)
        kmeans.fit(trial_features)
        wcss.append(kmeans.inertia_)
        
        explained_var = 1 - (kmeans.inertia_ / total_ss)
        results.append(f"Clusters: {i}, WCSS: {kmeans.inertia_:.2f}, Explained Variability: {explained_var:.2%}")

    persistence.save_text("\n".join(results), path)