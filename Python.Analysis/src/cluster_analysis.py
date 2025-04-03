from pathlib import Path
from typing import List
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.patches import Circle
from sklearn.cluster import KMeans
from src.domain import Persistence, Trial, Condition

def perform_cluster_analysis(trials: List[Trial], n_clusters: int) -> None:
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            # trial.timing_between_last_touch_and_pass(),
            trial.average_interpersonal_distance(),
            # trial.interpersonal_distance_at_pass_time(),
            trial.number_lateral_changes_of_direction()
        ]
        for trial in trials
    ])
    
    kmeans = KMeans(n_clusters=n_clusters)
    kmeans.fit(trial_features)
    labels = kmeans.labels_

    # Store the cluster label in the trial object
    for trial, label in zip(trials, labels):
        trial.cluster_label = label

    # Calculate and print percentage of trials in each cluster per condition
    conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
    print("\nPercentage of trials in each cluster by condition:")
    
    for condition in conditions:
        condition_trials = [trial for trial in trials if trial.condition == condition]
        condition_labels = [trial.cluster_label for trial in condition_trials]
        
        print(f"\nCondition: {condition.value}")
        for i in range(n_clusters):
            cluster_percentage = np.sum(np.array(condition_labels) == i) / len(condition_labels) * 100 if condition_labels else 0
            print(f'Cluster {i}: {cluster_percentage:.2f}%')

    # Calculate and print average distribution of trials in each cluster
    average_distribution = np.array([np.sum(labels == i) for i in range(n_clusters)]) / len(labels) * 100
    print("\nAverage distribution of trials in each cluster:")
    for i, avg in enumerate(average_distribution):
        print(f'Cluster {i}: {avg:.2f}%')

def plot_elbow_method(trials: List[Trial], max_clusters: int, persistence: Persistence) -> None:
    wcss = []
    trial_features = np.array([
        [
            trial.distance_between_last_touch_and_pass(),
            # trial.timing_between_last_touch_and_pass(),
            trial.average_interpersonal_distance(),
            # trial.interpersonal_distance_at_pass_time(),
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

def plot_cluster_distribution(trials: List[Trial], persistence: Persistence) -> None:
    conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
    cluster_distributions = {condition: [] for condition in conditions}

    # Collect cluster labels for each condition
    for trial in trials:
        cluster_distributions[trial.condition].append(trial.cluster_label)

    # Create a figure with 4 subplots for each condition
    fig, axes = plt.subplots(1, 4, figsize=(20, 5))
    fig.suptitle('Cluster Distribution by Condition')

    # Define a color cycler with white, black, and grey
    colors = ['#FFFFFF', '#000000', '#808080']  # White, Black, Grey

    for ax, condition in zip(axes, conditions):
        labels, counts = np.unique(cluster_distributions[condition], return_counts=True)
        sizes = counts / counts.sum() * 100  # Convert counts to percentages
        
        wedges, texts, autotexts = ax.pie(sizes, labels=[f'Cluster {label}' for label in labels], 
                                            autopct='%1.1f%%', startangle=90, colors=colors,
                                            wedgeprops=dict(linewidth=2, edgecolor='black'))  # Add outline properties
        
        # Add a circle outline around the pie chart
        centre_circle = Circle((0, 0), 0.70, fc='white')  # Create a white circle in the center
        ax.add_artist(centre_circle)  # Add the circle to the pie chart
        ax.axis('equal')  # Equal aspect ratio ensures that pie is drawn as a circle
        ax.set_title(condition.value)

    plt.tight_layout()
    plt.subplots_adjust(top=0.85)  # Adjust the top to make room for the title

    # Save the plot using the persistence object
    persistence.save_figure(plt, Path("cluster_distribution_plot.png"))  # Adjust the filename as needed
    plt.savefig("cluster_distribution_plot.png")  # Save the plot locally
    plt.close()  # Close the plot to free memory 