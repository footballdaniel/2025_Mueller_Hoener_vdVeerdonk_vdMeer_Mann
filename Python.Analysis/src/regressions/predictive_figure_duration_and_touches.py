from pathlib import Path
import re
import numpy as np
import arviz as az
import matplotlib.pyplot as plt
import seaborn as sns
from ..domain import Condition
from ..persistence import ColumnFormat, Persistence, ApaStyledPersistence

def duration_and_touches_predictive_figure(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))

    duration_samples = duration_results.posterior["1|condition"].values
    touches_samples = touches_results.posterior["1|condition"].values

    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1]).T
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1]).T

    duration_means = duration_samples.mean(axis=1)
    touches_means = touches_samples.mean(axis=1)

    duration_lower = np.percentile(duration_samples, 2.5, axis=1)
    duration_upper = np.percentile(duration_samples, 97.5, axis=1)
    touches_lower = np.percentile(touches_samples, 2.5, axis=1)
    touches_upper = np.percentile(touches_samples, 97.5, axis=1)

    conditions = [c.value for c in reversed(Condition)]
    
    interval_results = "95% Credible Intervals for Duration and Touches\n"
    interval_results += "===========================================\n\n"
    
    interval_results += "Duration Intervals:\n"
    interval_results += "-----------------\n"
    for condition, (lower, upper) in zip(conditions, zip(duration_lower, duration_upper)):
        width = upper - lower
        interval_results += f"{condition}:\n"
        interval_results += f"  Mean: {duration_means[conditions.index(condition)]:.3f} seconds\n"
        interval_results += f"  95% CI: [{lower:.3f}, {upper:.3f}] seconds\n"
        interval_results += f"  Width: {width:.3f} seconds\n\n"
    
    interval_results += "Touches Intervals:\n"
    interval_results += "----------------\n"
    for condition, (lower, upper) in zip(conditions, zip(touches_lower, touches_upper)):
        width = upper - lower
        interval_results += f"{condition}:\n"
        interval_results += f"  Mean: {touches_means[conditions.index(condition)]:.3f} touches\n"
        interval_results += f"  95% CI: [{lower:.3f}, {upper:.3f}] touches\n"
        interval_results += f"  Width: {width:.3f} touches\n\n"

    if isinstance(persistence, ApaStyledPersistence):
        persistence.save_text(interval_results, file_name.with_suffix('.txt'))
    else:
        print(interval_results)

    x_pos = np.arange(len(conditions))

    plt.figure(figsize=(12, 6))
    plt.bar(x_pos - 0.2, duration_means, width=0.4, label='Duration', color='b')
    plt.bar(x_pos + 0.2, touches_means, width=0.4, label='Touches', color='r')
    plt.axhline(0, color='k', linewidth=0.5)
    plt.ylabel('Mean')
    plt.xlabel('Condition')
    plt.title('Predictive Means for Duration and Touches')
    plt.xticks(x_pos, conditions)
    plt.legend()
    plt.show()

    plt.figure(figsize=(12, 6))
    plt.bar(x_pos - 0.2, duration_lower, width=0.4, label='Duration Lower', color='b')
    plt.bar(x_pos + 0.2, duration_upper, width=0.4, label='Duration Upper', color='b', bottom=duration_lower)
    plt.bar(x_pos - 0.2, touches_lower, width=0.4, label='Touches Lower', color='r')
    plt.bar(x_pos + 0.2, touches_upper, width=0.4, label='Touches Upper', color='r', bottom=touches_lower)
    plt.axhline(0, color='k', linewidth=0.5)
    plt.ylabel('Credible Interval')
    plt.xlabel('Condition')
    plt.title('Predictive Credible Intervals for Duration and Touches')
    plt.xticks(x_pos, conditions)
    plt.legend()
    plt.show() 