from src.domain import Condition


import matplotlib.pyplot as plt


from pathlib import Path


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