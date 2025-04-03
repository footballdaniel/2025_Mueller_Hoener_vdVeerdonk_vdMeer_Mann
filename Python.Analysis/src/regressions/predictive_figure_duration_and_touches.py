from pathlib import Path
import re
import numpy as np
import arviz as az
import matplotlib.pyplot as plt
import seaborn as sns
from ..domain import Condition

from ..persistence import ColumnFormat, Persistence

def duration_and_touches_predictive_figure(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))

    duration_samples = duration_results.posterior["C(condition)"].values
    touches_samples = touches_results.posterior["C(condition)"].values

    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1]).T
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1]).T

    duration_means = duration_samples.mean(axis=1)
    touches_means = touches_samples.mean(axis=1)

    conditions = [c.value for c in Condition]
    x_pos = np.arange(len(conditions))

    fig, ax1 = plt.subplots(figsize=(persistence.figure_width(ColumnFormat.DOUBLE), 2.75))
    ax2 = ax1.twinx()

    ax1.bar(x_pos - 0.2, duration_means, 0.4, capsize=5, color='#4A90E2', label='Duration')
    ax1.set_ylabel('Duration [s]')

    ax2.bar(x_pos + 0.2, touches_means, 0.4, capsize=5, color='#8B0000', label='Touches')
    ax2.set_ylabel('Number of Touches [n]')

    ax1.set_xticks(x_pos)
    formatted_labels = [re.sub(r'([a-z])([A-Z])', r'\1 \2', label) for label in conditions]
    ax1.set_xticklabels(formatted_labels, rotation=0)

    for i in range(len(conditions)):
        duration_jitter = np.random.normal(0, 0.05, size=duration_samples.shape[1])
        touches_jitter = np.random.normal(0, 0.05, size=touches_samples.shape[1])
        ax1.scatter(i - 0.2 + duration_jitter, duration_samples[i], alpha=0.1, color='black', s=10)
        ax2.scatter(i + 0.2 + touches_jitter, touches_samples[i], alpha=0.1, color='black', s=10)

    lines1, labels1 = ax1.get_legend_handles_labels()
    lines2, labels2 = ax2.get_legend_handles_labels()
    ax1.legend(lines1 + lines2, labels1 + labels2, loc='upper left')

    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig)