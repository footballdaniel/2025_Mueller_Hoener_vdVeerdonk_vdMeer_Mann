import os
from pathlib import Path
from typing import List
import re

import arviz as az
import bambi as bmb
import numpy as np
import pandas as pd
import seaborn as sns
from matplotlib import pyplot as plt

from .domain import TrialCollection, Condition
from .persistence import ColumnFormat, Persistence, Table


def regression_duration(trials: TrialCollection, model_path: Path, model_description_path: Path, persistence: Persistence) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials.trials],
        "duration": [trial.duration() for trial in trials.trials],
        "condition": [trial.condition.value for trial in trials.trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    
    # Define conditions in desired order
    condition_order = [c.value for c in Condition]
    condition_order.reverse()
    df["condition"] = pd.Categorical(df["condition"], categories=condition_order, ordered=True)
    df["condition_idx"] = df["condition"].cat.codes
    
    # Priors for fixed effects
    numeric_prior = bmb.Prior("Uniform", lower=0, upper=10)
    
    # Priors for random effects with partial pooling
    participant_prior = bmb.Prior(
        "Normal",
        mu=0,
        sigma=bmb.Prior("HalfNormal", sigma=1.0)
    )
    
    # Prior for observation noise
    observation_noise = bmb.Prior("HalfNormal", sigma=2.5)

    priors = {
        "C(condition)": numeric_prior,
        "sigma": observation_noise,
        "1|participant_id": participant_prior
    }
    formula = "duration ~ 0 + C(condition) + (1|participant_id)"
    
    model_bambi = bmb.Model(
        formula,
        df,
        family="gaussian",
        priors=priors,
    )
    
    persistence.save_model(model_bambi.__str__(), model_description_path)
    
    if not os.path.exists(str(model_path)):
        results = model_bambi.fit(
            draws=100,
            chains=4,
            idata_kwargs={"log_likelihood": True},
            random_seed=1991
        )
        
        model_bambi.predict(results, kind="response")
        
        az.to_netcdf(results, str(model_path))



def table_duration(inference_data: Path, file_name: Path, persistence: Persistence) -> None:
    results = az.from_netcdf(str(inference_data))
    
    summary_predictors = az.summary(
        results,
        hdi_prob=0.95,
        var_names=["~^p$|participant|^mu"],  # Fixed regex pattern
        filter_vars="regex"
    )
    
    summary_varying_intercepts_sigma = az.summary(
        results,
        hdi_prob=0.95,
        var_names=["sigma"],
        filter_vars="like"
    )
    
    summary = pd.concat([summary_predictors, summary_varying_intercepts_sigma], axis=0)
    
    summary_df = summary[["mean", "sd", "hdi_2.5%", "hdi_97.5%", "ess_bulk"]].copy()
    summary_df = summary_df.round(2)
    summary_df["ess_bulk"] = summary_df["ess_bulk"].astype(int)
    
    summary_df["CI"] = summary_df.apply(
        lambda row: f"{row['hdi_2.5%']:.2f} – {row['hdi_97.5%']:.2f}", axis=1
    )
    
    summary_df.insert(0, 'Predictors', summary.index)
    
    rows = summary_df[["Predictors", "mean", "CI", "ess_bulk"]].astype(str).values.tolist()
    
    header = ["Predictors", "Estimates", "CI (2.5%, 97.5%)", "ESS"]
    
    table = Table(
        title="Hierarchical regression model predictions per condition",
        header=header,
        rows=rows
    )
    
    table.rename_element("0.0", "<0.01")
    table.rename_element("Intercept", "Participant-specific intercept αj")
    table.rename_element("condition_idx[0]", "InSitu")
    table.rename_element("condition_idx[1]", "Interaction")
    table.rename_element("condition_idx[2]", "NoInteraction")
    table.rename_element("condition_idx[3]", "NoOpponent")
    table.rename_element("1|participant_sigma", "σ Participant")
    
    persistence.save_table(table, file_name)


def regression_touches(trials: TrialCollection, model_path: Path, model_description_path: Path, persistence: Persistence) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials.trials],
        "touches": [trial.number_of_touches() for trial in trials.trials],
        "condition": [trial.condition.value for trial in trials.trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    
    # Define conditions in desired order
    condition_order = [c.value for c in Condition]
    condition_order.reverse()
    df["condition"] = pd.Categorical(df["condition"], categories=condition_order, ordered=True)
    df["condition_idx"] = df["condition"].cat.codes
    
    # Priors for fixed effects
    numeric_prior = bmb.Prior("Uniform", lower=0, upper=10)
    
    # Priors for random effects with partial pooling
    participant_prior = bmb.Prior(
        "Normal",
        mu=0,
        sigma=bmb.Prior("HalfNormal", sigma=1.0)
    )
    
    # Prior for observation noise
    observation_noise = bmb.Prior("HalfNormal", sigma=2.5)

    priors = {
        "C(condition)": numeric_prior,
        "sigma": observation_noise,
        "1|participant_id": participant_prior
    }
    formula = "touches ~ 0 + C(condition) + (1|participant_id)"
    
    model_bambi = bmb.Model(
        formula,
        df,
        family="gaussian",
        priors=priors,
    )
    
    persistence.save_model(model_bambi.__str__(), model_description_path)
    
    if not os.path.exists(str(model_path)):
        results = model_bambi.fit(
            draws=100,
            chains=4,
            idata_kwargs={"log_likelihood": True},
            random_seed=1991
        )
        
        model_bambi.predict(results, kind="response")
        
        az.to_netcdf(results, str(model_path))


def duration_and_touches_figure(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    duration_samples = duration_results.posterior["C(condition)"].values
    touches_samples = touches_results.posterior["C(condition)"].values
    
    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1])
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1])
    
    duration_samples = duration_samples.T
    touches_samples = touches_samples.T
    
    duration_means = duration_samples.mean(axis=1)
    touches_means = touches_samples.mean(axis=1)
    
    conditions = [c.value for c in Condition]
    x_pos = np.arange(len(conditions))
    
    fig, ax1 = plt.subplots(figsize=(persistence.figure_width(ColumnFormat.DOUBLE), 2.75))
    ax2 = ax1.twinx()
    
    # Plot duration on left y-axis (first)
    ax1.bar(x_pos - 0.2, duration_means, 0.4, capsize=5, color='#4A90E2', label='Duration')
    ax1.set_ylabel('Duration [s]')
    
    # Plot touches on right y-axis (second)
    ax2.bar(x_pos + 0.2, touches_means, 0.4, capsize=5, color='#8B0000', label='Touches')
    ax2.set_ylabel('Number of Touches [n]')
    
    # Set x-axis with formatted labels
    ax1.set_xticks(x_pos)
    formatted_labels = [re.sub(r'([a-z])([A-Z])', r'\1 \2', label) for label in conditions]
    ax1.set_xticklabels(formatted_labels, rotation=0)
    
    # Add individual points with black jitter
    for i in range(len(conditions)):
        duration_jitter = np.random.normal(0, 0.05, size=duration_samples.shape[1])
        touches_jitter = np.random.normal(0, 0.05, size=touches_samples.shape[1])
        ax1.scatter(i - 0.2 + duration_jitter, duration_samples[i], alpha=0.1, color='black', s=10)
        ax2.scatter(i + 0.2 + touches_jitter, touches_samples[i], alpha=0.1, color='black', s=10)
    
    # Add legend in top left
    lines1, labels1 = ax1.get_legend_handles_labels()
    lines2, labels2 = ax2.get_legend_handles_labels()
    ax1.legend(lines1 + lines2, labels1 + labels2, loc='upper left')
    
    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig)

def probability_effect_is_zero(x):
    prob = max((x > 0).mean(), (x < 0).mean())
    rounded_prob = round(prob, 2)
    return 1 - rounded_prob



def table_duration_and_touches(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    duration_summary = az.summary(
        duration_results,
        hdi_prob=0.95,
        stat_funcs={"Zero effect probability": probability_effect_is_zero},
        var_names=["~^mu"],
        filter_vars="regex"
    )
    
    touches_summary = az.summary(
        touches_results,
        hdi_prob=0.95,
        stat_funcs={"Zero effect probability": probability_effect_is_zero},
        var_names=["~^mu"],
        filter_vars="regex"
    )
    
    duration_summary = duration_summary[~duration_summary.index.str.contains(r'1\|participant_id\[\d+\]')]
    touches_summary = touches_summary[~touches_summary.index.str.contains(r'1\|participant_id\[\d+\]')]
    
    # Reorder variables for each metric
    def reorder_summary(summary: pd.DataFrame) -> pd.DataFrame:
        # Get the order of variables we want
        desired_order = [
            "sigma",
            "1|participant_id_sigma",
            "C(condition)[InSitu]",
            "C(condition)[Interaction]",
            "C(condition)[NoInteraction]",
            "C(condition)[NoOpponent]",
            "0.0"
        ]
        # Filter to only include variables that exist in the summary
        existing_vars = [var for var in desired_order if var in summary.index]
        # Reorder the summary
        return summary.loc[existing_vars]
    
    duration_summary = reorder_summary(duration_summary)
    touches_summary = reorder_summary(touches_summary)
    
    for summary in [duration_summary, touches_summary]:
        summary[["mean", "sd", "hdi_2.5%", "hdi_97.5%", "ess_bulk"]] = summary[["mean", "sd", "hdi_2.5%", "hdi_97.5%", "ess_bulk"]].round(2)
        summary["ess_bulk"] = summary["ess_bulk"].astype(int)
        summary["CI"] = summary.apply(
            lambda row: f"{row['hdi_2.5%']:.2f} – {row['hdi_97.5%']:.2f}", axis=1
        )
    
    duration_summary.insert(0, 'Metric', 'Duration')
    touches_summary.insert(0, 'Metric', 'Touches')
    
    combined_summary = pd.concat([duration_summary, touches_summary], axis=0)
    combined_summary.insert(0, 'Predictors', combined_summary.index)
    
    rows = combined_summary[["Metric", "Predictors", "mean", "CI", "ess_bulk", "Zero effect probability"]].astype(str).values.tolist()
    
    header = ["Metric", "Predictors", "Estimates", "CI (2.5%, 97.5%)", "ESS", "Zero effect probability"]
    
    table = Table(
        title="Combined hierarchical regression model predictions per condition",
        header=header,
        rows=rows
    )
    
    # Rename elements for both metrics
    for metric in ["Duration", "Touches"]:
        table.rename_element("0.0", "<0.01")
        table.rename_element("C(condition)[NoOpponent]", "α[No Opponent]")
        table.rename_element("C(condition)[NoInteraction]", "α[No Interaction]")
        table.rename_element("C(condition)[Interaction]", "α[Interaction]")
        table.rename_element("C(condition)[InSitu]", "α[In Situ]")
        table.rename_element("1|participant_id_sigma", "σ Participant")
        table.rename_element("sigma", "σ")
    
    persistence.save_table(table, file_name)


def calculate_condition_differences(model_path: Path, variable_name: str) -> pd.DataFrame:
    results = az.from_netcdf(str(model_path))
    samples = results.posterior["C(condition)"].values
    samples = samples.reshape(-1, samples.shape[-1])
    
    conditions = [c.value for c in Condition]
    conditions.reverse()
    
    # Calculate all pairwise differences
    differences = []
    for i in range(len(conditions)):
        for j in range(i + 1, len(conditions)):
            diff = samples[:, j] - samples[:, i]
            prob_greater = np.mean(diff > 0)
            mean_diff = np.mean(diff)
            hdi_low, hdi_high = az.hdi(diff, hdi_prob=0.95)
            
            differences.append({
                'comparison': f'{conditions[j]} vs {conditions[i]}',
                'probability_greater': prob_greater,
                'mean_difference': mean_diff,
                'hdi_low': hdi_low,
                'hdi_high': hdi_high
            })
    
    return pd.DataFrame(differences)


def duration_and_touches_figure_post_hoc(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
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
    ax1.set_title("Duration Differences Between Conditions", pad=20)
    ax1.set_xlabel("")
    
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
    ax2.set_title("Differences in Number of Touches Between Conditions", pad=20)
    ax2.set_xlabel("")
    
    # Ensure tight layout
    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig) 