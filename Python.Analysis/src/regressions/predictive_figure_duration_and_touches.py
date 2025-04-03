from pathlib import Path
import numpy as np
import arviz as az
import matplotlib.pyplot as plt
import seaborn as sns

from ..persistence import Persistence


def predictive_figure_duration_and_touches(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    # Debug: Print the structure of the ArviZ objects
    print("\nDuration Results Structure:")
    print("Groups:", duration_results.groups())
    print("\nPosterior Predictive Variables:", duration_results.posterior_predictive.data_vars)
    print("\nPosterior Variables:", duration_results.posterior.data_vars)
    
    print("\nTouches Results Structure:")
    print("Groups:", touches_results.groups())
    print("\nPosterior Predictive Variables:", touches_results.posterior_predictive.data_vars)
    print("\nPosterior Variables:", touches_results.posterior.data_vars)
    
    # Get posterior predictive samples and reshape to (n_samples, n_conditions)
    duration_samples = duration_results.posterior_predictive["duration"].values
    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1])
    
    touches_samples = touches_results.posterior_predictive["touches"].values
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1])
    
    # Calculate means and standard errors
    duration_means = np.mean(duration_samples, axis=0)
    duration_std = np.std(duration_samples, axis=0)
    touches_means = np.mean(touches_samples, axis=0)
    touches_std = np.std(touches_samples, axis=0)
    
    # Create figure with dual y-axes
    fig, ax1 = plt.subplots(figsize=(10, 6))
    ax2 = ax1.twinx()
    
    # Plot duration on left y-axis
    duration_bars = ax1.bar(
        np.arange(len(duration_means)),
        duration_means,
        yerr=duration_std,
        capsize=5,
        color='blue',
        alpha=0.7,
        label='Duration'
    )
    
    # Plot touches on right y-axis
    touches_bars = ax2.bar(
        np.arange(len(touches_means)),
        touches_means,
        yerr=touches_std,
        capsize=5,
        color='red',
        alpha=0.7,
        label='Touches'
    )
    
    # Add jitter points for individual data
    for i in range(len(duration_means)):
        ax1.scatter(
            [i] * len(duration_samples),
            duration_samples[:, i],
            color='blue',
            alpha=0.1,
            s=10
        )
        ax2.scatter(
            [i] * len(touches_samples),
            touches_samples[:, i],
            color='red',
            alpha=0.1,
            s=10
        )
    
    # Set x-axis labels
    conditions = ["In Situ", "Interaction", "No Interaction", "No Opponent"]
    ax1.set_xticks(np.arange(len(conditions)))
    ax1.set_xticklabels(conditions)
    
    # Set y-axis labels
    ax1.set_ylabel('Duration (seconds)', color='blue')
    ax2.set_ylabel('Number of Touches', color='red')
    
    # Add legend
    lines1, labels1 = ax1.get_legend_handles_labels()
    lines2, labels2 = ax2.get_legend_handles_labels()
    ax1.legend(lines1 + lines2, labels1 + labels2, loc='upper left')
    
    # Set title
    plt.title('Predictive Distributions of Duration and Touches by Condition')
    
    # Adjust layout
    plt.tight_layout()
    
    # Save figure
    persistence.save_figure(fig, file_name) 