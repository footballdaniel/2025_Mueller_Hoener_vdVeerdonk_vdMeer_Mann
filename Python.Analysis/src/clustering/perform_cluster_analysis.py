from typing import List
import numpy as np
from sklearn.cluster import KMeans
from src.domain import Condition, TrialCollection
import pandas as pd
from src.persistence import Persistence
from src.persistence.apa_word_table_formatter import Table
from pathlib import Path

from ..services import DistanceCalculator, TimeCalculator, MovementCalculator


def perform_cluster_analysis(trials: TrialCollection, n_clusters: int, persistence: Persistence, file_name: Path) -> None:
    trial_features = np.array([
        [
            DistanceCalculator.distance_between_last_touch_and_pass(trial),
            TimeCalculator.time_between_last_change_of_direction_and_pass(trial),
            MovementCalculator.number_lateral_changes_of_direction(trial)
        ]
        for trial in trials
    ])

    kmeans = KMeans(n_clusters=n_clusters, random_state=1991)
    kmeans.fit(trial_features)
    labels = kmeans.labels_

    # Store the cluster label in the trial object, adding 1 to make clusters start from 1
    for trial, label in zip(trials, labels):
        trial.cluster_label = label + 1

    # Calculate cluster distribution per condition
    conditions = [Condition.InSitu, Condition.Interaction, Condition.NoInteraction, Condition.NoOpponent]
    condition_distributions = {}
    
    for condition in conditions:
        condition_trials = [trial for trial in trials if trial.condition == condition]
        condition_labels = [trial.cluster_label for trial in condition_trials]
        total_trials = len(condition_labels)
        
        cluster_counts = [condition_labels.count(i) for i in range(1, n_clusters + 1)]
        cluster_percentages = [(count / total_trials * 100) if total_trials > 0 else 0 for count in cluster_counts]
        
        condition_distributions[condition.value] = {
            'counts': cluster_counts,
            'percentages': cluster_percentages,
            'total': total_trials
        }

    # Write condition distributions to text file
    with open("cluster_distribution.txt", "w") as f:
        f.write("Cluster Distribution by Condition\n")
        f.write("===============================\n\n")
        
        for condition in conditions:
            dist = condition_distributions[condition.value]
            f.write(f"{condition.value} (N = {dist['total']}):\n")
            
            for i in range(n_clusters):
                f.write(f"  Cluster {i + 1}: {dist['counts'][i]} trials ({dist['percentages'][i]:.1f}%)\n")
            
            f.write("\n")

    # Calculate cluster percentages and counts for overall distribution
    total_trials = len(trials)
    cluster_counts = np.array([np.sum(labels == i) for i in range(n_clusters)])
    cluster_percentages = (cluster_counts / total_trials * 100).round(2)

    # Calculate average feature values per cluster
    cluster_averages = {i + 1: [] for i in range(n_clusters)}
    feature_names = [
        "Distance between last touch and pass [m]",
        "Time between last change of direction and pass [s]",
        "Number of lateral changes of direction [N]"
    ]

    for i in range(n_clusters):
        cluster_trials = [trial_features[j] for j in range(len(trials)) if labels[j] == i]
        if cluster_trials:
            cluster_averages[i + 1] = np.mean(cluster_trials, axis=0)
        else:
            cluster_averages[i + 1] = [float('nan')] * trial_features.shape[1]

    # Create table rows
    rows = []
    for cluster in range(1, n_clusters + 1):
        # Add first row with cluster number and first feature
        rows.append([
            f"Cluster {cluster}",
            feature_names[0],
            f"{cluster_averages[cluster][0]:.2f}"
        ])
        
        # Add second row with percentage and count
        rows.append([
            f"({cluster_percentages[cluster-1]}% of trials, N = {cluster_counts[cluster-1]})",
            feature_names[1],
            f"{cluster_averages[cluster][1]:.2f}"
        ])
        
        # Add third row with last feature
        rows.append([
            "",
            feature_names[2],
            f"{cluster_averages[cluster][2]:.2f}"
        ])

    # Create and save table
    table = Table(
        title="Average Feature Values per Cluster",
        header=["Cluster", "Feature", "Average Value"],
        rows=rows
    )
    
    persistence.save_table(table, file_name)


