from src.domain import Condition, TrialCollection


import numpy as np
import pandas as pd
from sklearn.cluster import KMeans


def perform_cluster_analysis(trials: TrialCollection, n_clusters: int) -> None:
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            trial.time_between_last_change_of_direction_and_pass(),
            trial.number_lateral_changes_of_direction()
        ]
        for trial in trials
    ])

    kmeans = KMeans(n_clusters=n_clusters, random_state=1991)
    kmeans.fit(trial_features)
    labels = kmeans.labels_

    # Store the cluster label in the trial object
    for trial, label in zip(trials, labels):
        trial.cluster_label = label

    # Prepare to collect averages for each condition
    conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
    averages = {condition.value: [] for condition in conditions}

    # Calculate and print percentage of trials in each cluster per condition
    print("\nPercentage of trials in each cluster by condition:")

    for condition in conditions:
        condition_trials = [trial for trial in trials if trial.condition == condition]
        condition_labels = [trial.cluster_label for trial in condition_trials]

        print(f"\nCondition: {condition.value}")
        for i in range(n_clusters):
            cluster_percentage = np.sum(np.array(condition_labels) == i) / len(
                condition_labels) * 100 if condition_labels else 0
            print(f'Cluster {i}: {cluster_percentage:.2f}%')

        # Calculate averages for the remaining features per condition
        if condition_trials:
            timings = np.array([trial.timing_between_last_touch_and_pass() for trial in condition_trials])
            average_timing = np.mean(timings) if len(timings) > 0 else 0

            time_between_changes = np.array(
                [trial.time_between_last_change_of_direction_and_pass() for trial in condition_trials])
            average_time_between_changes = np.mean(time_between_changes) if len(time_between_changes) > 0 else 0

            lateral_changes = np.array([trial.number_lateral_changes_of_direction() for trial in condition_trials])
            average_lateral_changes = np.mean(lateral_changes) if len(lateral_changes) > 0 else 0

            # Store averages in the dictionary
            averages[condition.value] = [
                average_timing,
                average_time_between_changes,
                average_lateral_changes
            ]

    # Create a DataFrame to tabulate the averages with conditions as headers
    averages_df = pd.DataFrame(averages, index=[
        'Average Timing Between Last Touch and Pass',
        'Average Time Between Last Change of Direction and Pass',
        'Average Number of Lateral Changes of Direction'
    ])

    print("\nAverages of Features per Condition:")
    print(averages_df)

    # Calculate and print average distribution of trials in each cluster
    average_distribution = np.array([np.sum(labels == i) for i in range(n_clusters)]) / len(labels) * 100
    print("\nAverage distribution of trials in each cluster:")
    for i, avg in enumerate(average_distribution):
        print(f'Cluster {i}: {avg:.2f}%')

    # Calculate and print average feature values per cluster
    print("\nAverage Feature Values per Cluster:")
    cluster_averages = {i: [] for i in range(n_clusters)}

    for i in range(n_clusters):
        cluster_trials = [trial_features[j] for j in range(len(trials)) if labels[j] == i]
        if cluster_trials:
            cluster_averages[i] = np.mean(cluster_trials, axis=0)
        else:
            cluster_averages[i] = [float('nan')] * trial_features.shape[1]  # Handle empty clusters

    # Print the average feature values for each cluster
    for i in range(n_clusters):
        print(f"\nCluster {i}:")
        print(f"  Average Timing Between Last Touch and Pass: {cluster_averages[i][0]:.2f}")
        print(f"  Average Time Between Last Change of Direction and Pass: {cluster_averages[i][1]:.2f}")
        print(f"  Average Number of Lateral Changes of Direction: {cluster_averages[i][2]:.2f}")