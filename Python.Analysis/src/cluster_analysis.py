from pathlib import Path
from typing import List
import numpy as np
import matplotlib.pyplot as plt
from sklearn.cluster import KMeans
from src.domain import Persistence, Trial

def perform_cluster_analysis(trials: List[Trial], n_clusters: int) -> None:
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            trial.timing_between_last_touch_and_pass(),
            trial.average_interpersonal_distance(),
            trial.interpersonal_distance_at_pass_time(),
            trial.number_lateral_changes_of_direction()
        ]
        for trial in trials
    ])
    
    kmeans = KMeans(n_clusters=n_clusters)
    kmeans.fit(trial_features)
    labels = kmeans.labels_

    for trial, label in zip(trials, labels):
        print(f'Trial ID: {trial.participant_id}, Cluster: {label}')

def plot_elbow_method(trials: List[Trial], max_clusters: int, persistence: Persistence) -> None:
    wcss = []
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            trial.timing_between_last_touch_and_pass(),
            trial.average_interpersonal_distance(),
            trial.interpersonal_distance_at_pass_time(),
            trial.number_lateral_changes_of_direction()
        ]
        for trial in trials
    ])
    
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
    persistence.save_figure(plt,Path("elbow_method_plot.png"))  # Adjust the filename as needed
    plt.savefig("elbow_method_plot.png")  # Save the plot locally
    plt.close()  # Close the plot to free memory 