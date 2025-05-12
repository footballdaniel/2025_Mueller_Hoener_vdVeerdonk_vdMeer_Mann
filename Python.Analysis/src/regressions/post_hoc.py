from pathlib import Path
import numpy as np
import arviz as az
import matplotlib.pyplot as plt
import seaborn as sns
from matplotlib.font_manager import FontProperties

from ..domain import Condition
import re

from ..persistence import ColumnFormat, Persistence

def duration_and_touches_post_hoc(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    # Get samples for both variables
    duration_samples = duration_results.posterior["C(condition)"].values
    touches_samples = touches_results.posterior["C(condition)"].values
    
    # Reshape samples
    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1])
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1])
    
    # Get conditions in reverse order
    conditions = [c.value for c in Condition]
    conditions.reverse()
    
    # Create figure with two subplots
    fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(persistence.figure_width(ColumnFormat.DOUBLE), 3.5))
    
    # Calculate duration differences
    duration_diffs = {}
    for i in range(len(conditions)):
        for j in range(i + 1, len(conditions)):
            diff = duration_samples[:, j] - duration_samples[:, i]
            condition_j = re.sub(r'([a-z])([A-Z])', r'\1 \2', conditions[j])
            condition_i = re.sub(r'([a-z])([A-Z])', r'\1 \2', conditions[i])
            comparison = f'{condition_j} vs {condition_i}'
            duration_diffs[comparison] = diff
    
    # Calculate touches differences
    touches_diffs = {}
    for i in range(len(conditions)):
        for j in range(i + 1, len(conditions)):
            diff = touches_samples[:, j] - touches_samples[:, i]
            condition_j = re.sub(r'([a-z])([A-Z])', r'\1 \2', conditions[j])
            condition_i = re.sub(r'([a-z])([A-Z])', r'\1 \2', conditions[i])
            comparison = f'{condition_j} vs {condition_i}'
            touches_diffs[comparison] = diff
    
    # Sort differences by effect size (mean absolute difference)
    duration_diffs = dict(sorted(
        duration_diffs.items(),
        key=lambda x: abs(np.mean(x[1]))
    ))
    touches_diffs = dict(sorted(
        touches_diffs.items(),
        key=lambda x: abs(np.mean(x[1]))
    ))
    
    # Create ArviZ InferenceData objects for differences
    duration_diff_data = az.convert_to_inference_data(
        {k: v.reshape(1, -1) for k, v in duration_diffs.items()},
        group="posterior"
    )
    touches_diff_data = az.convert_to_inference_data(
        {k: v.reshape(1, -1) for k, v in touches_diffs.items()},
        group="posterior"
    )
    
    # Plot duration differences
    az.plot_forest(
        duration_diff_data,
        kind="forestplot",
        var_names=list(duration_diffs.keys()),
        linewidth=1,
        combined=True,
        hdi_prob=0.95,
        ax=ax1,
        colors="#4A90E2"
    )
    ax1.set_title("")
    ax1.set_xlabel("95% Difference Interval in Duration [s]")
    ax1.tick_params(axis='both', which='major', labelsize=plt.rcParams['font.size'])
    
    # Plot touches differences
    az.plot_forest(
        touches_diff_data,
        kind="forestplot",
        var_names=list(touches_diffs.keys()),
        linewidth=1,
        combined=True,
        hdi_prob=0.95,
        ax=ax2,
        colors="#8B0000"
    )
    ax2.set_title("")
    ax2.set_xlabel("95% Difference Interval in Number of Touches [N]")
    ax2.tick_params(axis='both', which='major', labelsize=plt.rcParams['font.size'])
    
    # Update font for all text elements
    font_prop = FontProperties(family=plt.rcParams['font.family'][0] if isinstance(plt.rcParams['font.family'], list) else plt.rcParams['font.family'])
    for ax in [ax1, ax2]:
        for item in ([ax.title, ax.xaxis.label, ax.yaxis.label] + ax.get_xticklabels() + ax.get_yticklabels()):
            item.set_fontproperties(font_prop)
    
    # Ensure tight layout
    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig) 