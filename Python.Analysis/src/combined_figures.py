import textwrap
from pathlib import Path

import arviz as az
import matplotlib.pyplot as plt
import numpy as np

from .domain import Condition
from .persistence import ColumnFormat, Persistence


def combined_predictive_and_cluster_figure(
        duration_model_path: Path,
        touches_model_path: Path,
        trials,
        file_name: Path,
        persistence: Persistence
) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))

    duration_samples = duration_results.posterior["C(condition)"].values
    touches_samples = touches_results.posterior["C(condition)"].values

    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1]).T
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1]).T

    duration_means = duration_samples.mean(axis=1)
    touches_means = touches_samples.mean(axis=1)

    duration_lower = np.percentile(duration_samples, 2.5, axis=1)
    duration_upper = np.percentile(duration_samples, 97.5, axis=1)
    touches_lower = np.percentile(touches_samples, 2.5, axis=1)
    touches_upper = np.percentile(touches_samples, 97.5, axis=1)

    conditions = list(Condition)
    x_positions = {
        str(Condition.NoOpponent): 1.0,
        str(Condition.NoInteraction): 2.5, 
        str(Condition.Interaction): 4.0,
        str(Condition.InSitu): 5.5
    }
    x_pos = np.array([x_positions[str(cond)] for cond in conditions])

    fig = plt.figure(figsize=(persistence.figure_width(ColumnFormat.DOUBLE), 4.5))
    fig.subplots_adjust(hspace=0.02, top=0.96, bottom=0.08)
    gs = fig.add_gridspec(2, 1, height_ratios=[1, 1])
    fig.subplots_adjust(hspace=0.05)

    ax1 = fig.add_subplot(gs[0])
    ax2 = ax1.twinx()

    ax1.spines['top'].set_visible(False)
    ax1.spines['left'].set_visible(False)
    ax2.spines['top'].set_visible(False)
    ax2.spines['right'].set_visible(False)

    ax1.bar(x_pos - 0.2, duration_means, 0.4,
            yerr=[duration_means - duration_lower, duration_upper - duration_means],
            capsize=5, color='#4A90E2', label='Trial duration')
    ax1.set_ylabel('Trial duration [s]')

    ax2.bar(x_pos + 0.2, touches_means, 0.4,
            yerr=[touches_means - touches_lower, touches_upper - touches_means],
            capsize=5, color='#8B0000', label='Number of touches')
    ax2.set_ylabel('Number of touches [N]')

    ax1.set_xticks(x_pos)
    formatted_labels = ['\n'.join(textwrap.wrap(str(cond), width=20)) for cond in conditions]
    ax1.set_xticklabels(formatted_labels, rotation=0)
    ax1.set_xticklabels(formatted_labels, rotation=0)

    legend_labels_top = ['Trial duration', 'Number of touches']
    legend_colors_top = ['#4A90E2', '#8B0000']
    legend_handles_top = [
        plt.Line2D([0], [0], marker='s', linestyle='None', color='none',
                   markerfacecolor=color, markeredgewidth=0, markersize=8)
        for color in legend_colors_top
    ]
    ax1.legend(
        legend_handles_top,
        legend_labels_top,
        loc='upper right',
        ncol=1,
        frameon=False
    )

    common_xlim = (0.5, 6.0)
    ax1.set_xlim(common_xlim)

    ax3 = fig.add_subplot(gs[1])
    ax3.set_anchor('N')

    colors = ['#000000', '#E0E0E0', '#808080']

    for condition in conditions:
        labels = [trial.cluster_label for trial in trials if trial.condition == condition]
        label_set = sorted(set(labels))
        counts = [labels.count(label) for label in label_set]
        total = sum(counts)
        sizes = [count / total * 100 for count in counts]
        cluster_colors = [colors[label % len(colors)] for label in label_set]

        pie_x = x_positions[str(condition)]
        pie_y = 0.5

        wedges, _ = ax3.pie(
            sizes,
            labels=None,
            autopct=None,
            startangle=90,
            colors=cluster_colors,
            wedgeprops=dict(linewidth=0, width=0.3),
            center=(pie_x, pie_y),
            radius=0.4
        )

        # Add percentage labels outside the pie slices
        for i, wedge in enumerate(wedges):
            angle = (wedge.theta2 - wedge.theta1) / 2. + wedge.theta1
            # Position labels outside the pie (radius > 0.4)
            x = pie_x + 0.45 * np.cos(np.deg2rad(angle))
            y = pie_y + 0.45 * np.sin(np.deg2rad(angle))
            # Add a small offset to prevent overlap
            if angle > 90 and angle < 270:  # Left side of pie
                x -= 0.1
            else:  # Right side of pie
                x += 0.1
            ax3.text(x, y, f"{int(round(sizes[i]))}%", ha='center', va='center', fontsize=8)


    legend_labels = [f'Cluster {i}' for i in range(3)]
    legend_labels_override = ["Minimal interactions", "Moderate interactions", "Strong interactions"]
    legend_colors = [colors[1], colors[2], colors[0]]
    legend_handles = [
        plt.Line2D([0], [0], marker='s', linestyle='None', color='none',
                   markerfacecolor=color, markeredgewidth=0, markersize=8)
        for color in legend_colors
    ]
    ax3.legend(
        legend_handles,
        legend_labels_override,
        loc='upper center',
        bbox_to_anchor=(0.5, +0.2),
        ncol=3,
        frameon=False
    )

    ax3.set_xlim(common_xlim)
    ax3.set_ylim(-0.5, 1.5)
    ax3.axis('off')

    persistence.save_figure(fig, file_name)
    plt.close(fig)
