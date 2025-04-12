from pathlib import Path
from typing import List
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.patches import Circle
from sklearn.cluster import KMeans
from src.domain import Persistence, Trial, Condition, TrialCollection
import pandas as pd  # Import pandas for tabular data handling
from collections import Counter


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


def plot_elbow_method(trials: TrialCollection, max_clusters: int, persistence: Persistence) -> None:
    wcss = []
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            trial.time_between_last_change_of_direction_and_pass(),
            trial.number_lateral_changes_of_direction()
        ]
        for trial in trials
    ])

    # Impute NaN values for time_between_last_change_of_direction_and_pass
    time_between_changes = trial_features[:, 1]  # Extract the second feature
    mean_time_between_changes = np.nanmean(time_between_changes)  # Calculate the mean, ignoring NaNs
    time_between_changes[np.isnan(time_between_changes)] = mean_time_between_changes  # Impute NaN values

    # Update trial_features with the imputed values
    trial_features[:, 1] = time_between_changes

    # Impute NaN values for other features if necessary
    for i in range(trial_features.shape[1]):
        mean_value = np.nanmean(trial_features[:, i])
        trial_features[np.isnan(trial_features[:, i]), i] = mean_value  # Impute NaN values

    for i in range(1, max_clusters + 1):
        kmeans = KMeans(n_clusters=i)
        kmeans.fit(trial_features)
        wcss.append(kmeans.inertia_)

    plt.figure(figsize=(10, 6))
    plt.plot(range(1, max_clusters + 1), wcss, marker='o')
    plt.title('Elbow Method for Optimal k')
    plt.xlabel('Number of clusters')
    plt.ylabel('WCSS')
    plt.xticks(range(1, max_clusters + 1))
    plt.grid()

    # Save the plot using the persistence object
    persistence.save_figure(plt, Path("elbow_method_plot.png"))  # Adjust the filename as needed
    plt.savefig("elbow_method_plot.png")  # Save the plot locally
    plt.close()  # Close the plot to free memory 


def plot_cluster_distribution(trials, persistence):
    conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
    fig, axes = plt.subplots(1, 4, figsize=(20, 5))
    fig.suptitle('Cluster Distribution by Condition')
    colors = ['#FFFFFF', '#000000', '#808080']

    for ax, condition in zip(axes, conditions):
        labels = [trial.cluster_label for trial in trials if
                  trial.condition == condition and trial.cluster_label is not None]
        label_set = set(labels)
        counts = [labels.count(label) for label in label_set]
        total = sum(counts)
        sizes = [count / total * 100 for count in counts]
        cluster_colors = [colors[label % len(colors)] for label in label_set]

        ax.pie(
            sizes,
            labels=[f'Cluster {label}' for label in label_set],
            autopct='%1.1f%%',
            startangle=90,
            colors=cluster_colors,
            wedgeprops=dict(linewidth=2, edgecolor='black')
        )

        centre_circle = plt.Circle((0, 0), 0.50, fc='white')
        ax.add_artist(centre_circle)
        ax.axis('equal')
        ax.set_title(condition.value)

    plt.tight_layout()
    plt.subplots_adjust(top=0.85)

    persistence.save_figure(plt, Path("cluster_distribution_plot.png"))
    plt.savefig("cluster_distribution_plot.png")
    plt.close()
