from pathlib import Path
import numpy as np
import arviz as az
import matplotlib.pyplot as plt
import seaborn as sns

from ..persistence import Persistence


def ridge_plot_conditions_and_differences(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    # Get posterior samples
    duration_samples = duration_results.posterior["duration"].values.reshape(-1)
    touches_samples = touches_results.posterior["touches"].values.reshape(-1)
    
    # Create figure with two subplots
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(10, 12))
    
    # Plot duration distributions
    for i, condition in enumerate(["In Situ", "Interaction", "No Interaction", "No Opponent"]):
        sns.kdeplot(
            data=duration_samples[:, i],
            ax=ax1,
            label=condition,
            fill=True,
            alpha=0.3
        )
        # Add vertical line for 95% HDI
        hdi = az.hdi(duration_samples[:, i], hdi_prob=0.95)
        ax1.axvline(hdi[0], color='gray', linestyle='--', alpha=0.5)
        ax1.axvline(hdi[1], color='gray', linestyle='--', alpha=0.5)
    
    # Plot touches distributions
    for i, condition in enumerate(["In Situ", "Interaction", "No Interaction", "No Opponent"]):
        sns.kdeplot(
            data=touches_samples[:, i],
            ax=ax2,
            label=condition,
            fill=True,
            alpha=0.3
        )
        # Add vertical line for 95% HDI
        hdi = az.hdi(touches_samples[:, i], hdi_prob=0.95)
        ax2.axvline(hdi[0], color='gray', linestyle='--', alpha=0.5)
        ax2.axvline(hdi[1], color='gray', linestyle='--', alpha=0.5)
    
    # Set titles and labels
    ax1.set_title('Duration Distributions by Condition')
    ax2.set_title('Number of Touches Distributions by Condition')
    ax1.set_xlabel('Duration (seconds)')
    ax2.set_xlabel('Number of Touches')
    ax1.set_ylabel('Density')
    ax2.set_ylabel('Density')
    
    # Add legends
    ax1.legend()
    ax2.legend()
    
    # Adjust layout
    plt.tight_layout()
    
    # Save figure
    persistence.save_figure(fig, file_name) 