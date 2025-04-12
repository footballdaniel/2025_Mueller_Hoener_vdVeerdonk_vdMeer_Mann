from src.domain import Persistence, TrialCollection


import matplotlib.pyplot as plt
import numpy as np
from sklearn.cluster import KMeans


from pathlib import Path


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