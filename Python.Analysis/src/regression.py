import os
from pathlib import Path
from typing import List

import arviz as az
import bambi as bmb
import numpy as np
import pandas as pd
import seaborn as sns
from matplotlib import pyplot as plt

from .domain import TrialCollection, Condition
from .persistence import Persistence, Table


def regression_duration(trials: TrialCollection, model_path: Path, model_description_path: Path, persistence: Persistence) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials.trials],
        "duration": [trial.duration() for trial in trials.trials],
        "condition": [trial.condition.value for trial in trials.trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    df["condition_idx"] = pd.Categorical(df["condition"]).codes
    
    numeric_prior = bmb.Prior("Uniform", lower=0, upper=10)
    flat_prior = bmb.Prior("Uniform", lower=-3, upper=3)
    normal_distribution = bmb.Prior("Normal", mu=0, sigma=2.5)
    half_normal_distribution = bmb.Prior("HalfNormal", sigma=2.5)

    priors = {
        "C(condition)": numeric_prior,
        "sigma": half_normal_distribution,
        "1|participant_id": half_normal_distribution
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


def predictive_figure_duration(inference_data: Path, file_name: Path, persistence: Persistence) -> None:
    results = az.from_netcdf(str(inference_data))
    samples = results.posterior["C(condition)"].values  # shape: (chains, draws, 4)
    samples = samples.reshape(-1, samples.shape[-1])    # shape: (n_samples, 4)
    samples = samples.T                                 # shape: (4, n_samples)

    condition_means = samples.mean(axis=1)
    condition_sems = samples.std(axis=1) / np.sqrt(samples.shape[1])

    conditions = [c.value for c in Condition]
    x_pos = np.arange(len(conditions))

    fig, ax = plt.subplots(figsize=(8, 6))
    ax.bar(x_pos, condition_means, yerr=condition_sems, capsize=5)
    ax.set_xticks(x_pos)
    ax.set_xticklabels(conditions, rotation=45)
    ax.set_ylabel('Duration (s)')
    ax.spines['top'].set_visible(False)
    ax.spines['right'].set_visible(False)

    for i in range(len(conditions)):
        jitter = np.random.normal(0, 0.05, size=samples.shape[1])
        ax.scatter(i + jitter, samples[i], alpha=0.1, color='black', s=10)

    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig)


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
    df["condition_idx"] = pd.Categorical(df["condition"]).codes
    
    numeric_prior = bmb.Prior("Uniform", lower=0, upper=10)
    flat_prior = bmb.Prior("Uniform", lower=-3, upper=3)
    normal_distribution = bmb.Prior("Normal", mu=0, sigma=2.5)
    half_normal_distribution = bmb.Prior("HalfNormal", sigma=2.5)

    priors = {
        "C(condition)": numeric_prior,
        "sigma": half_normal_distribution,
        "1|participant_id": half_normal_distribution
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


def predictive_figure_duration_and_touches(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    duration_samples = duration_results.posterior["C(condition)"].values
    touches_samples = touches_results.posterior["C(condition)"].values
    
    duration_samples = duration_samples.reshape(-1, duration_samples.shape[-1])
    touches_samples = touches_samples.reshape(-1, touches_samples.shape[-1])
    
    duration_samples = duration_samples.T
    touches_samples = touches_samples.T
    
    duration_means = duration_samples.mean(axis=1)
    duration_sems = duration_samples.std(axis=1) / np.sqrt(duration_samples.shape[1])
    touches_means = touches_samples.mean(axis=1)
    touches_sems = touches_samples.std(axis=1) / np.sqrt(touches_samples.shape[1])
    
    conditions = [c.value for c in Condition]
    x_pos = np.arange(len(conditions))
    
    fig, ax1 = plt.subplots(figsize=(8, 6))
    ax2 = ax1.twinx()
    
    # Plot duration on left y-axis
    ax1.bar(x_pos - 0.2, duration_means, 0.4, yerr=duration_sems, capsize=5, color='#4A90E2', label='Duration')
    ax1.set_ylabel('Duration (s)', color='#4A90E2')
    ax1.tick_params(axis='y', labelcolor='#4A90E2')
    
    # Plot touches on right y-axis
    ax2.bar(x_pos + 0.2, touches_means, 0.4, yerr=touches_sems, capsize=5, color='#8B0000', label='Touches')
    ax2.set_ylabel('Number of Touches', color='#8B0000')
    ax2.tick_params(axis='y', labelcolor='#8B0000')
    
    # Set x-axis
    ax1.set_xticks(x_pos)
    ax1.set_xticklabels(conditions, rotation=45)
    
    # Add individual points
    for i in range(len(conditions)):
        duration_jitter = np.random.normal(0, 0.05, size=duration_samples.shape[1])
        touches_jitter = np.random.normal(0, 0.05, size=touches_samples.shape[1])
        ax1.scatter(i - 0.2 + duration_jitter, duration_samples[i], alpha=0.1, color='#4A90E2', s=10)
        ax2.scatter(i + 0.2 + touches_jitter, touches_samples[i], alpha=0.1, color='#8B0000', s=10)
    
    # Add legend
    lines1, labels1 = ax1.get_legend_handles_labels()
    lines2, labels2 = ax2.get_legend_handles_labels()
    ax1.legend(lines1 + lines2, labels1 + labels2, loc='upper right')
    
    plt.tight_layout()
    persistence.save_figure(fig, file_name)
    plt.close(fig) 


def table_duration_and_touches(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    duration_summary = az.summary(
        duration_results,
        hdi_prob=0.95,
        var_names=["~^p$|participant|^mu"],
        filter_vars="regex"
    )
    
    touches_summary = az.summary(
        touches_results,
        hdi_prob=0.95,
        var_names=["~^p$|participant|^mu"],
        filter_vars="regex"
    )
    
    duration_sigma = az.summary(
        duration_results,
        hdi_prob=0.95,
        var_names=["sigma"],
        filter_vars="like"
    )
    
    touches_sigma = az.summary(
        touches_results,
        hdi_prob=0.95,
        var_names=["sigma"],
        filter_vars="like"
    )
    
    duration_summary = pd.concat([duration_summary, duration_sigma], axis=0)
    touches_summary = pd.concat([touches_summary, touches_sigma], axis=0)
    
    # Format both summaries
    for summary in [duration_summary, touches_summary]:
        summary[["mean", "sd", "hdi_2.5%", "hdi_97.5%", "ess_bulk"]] = summary[["mean", "sd", "hdi_2.5%", "hdi_97.5%", "ess_bulk"]].round(2)
        summary["ess_bulk"] = summary["ess_bulk"].astype(int)
        summary["CI"] = summary.apply(
            lambda row: f"{row['hdi_2.5%']:.2f} – {row['hdi_97.5%']:.2f}", axis=1
        )
    
    # Combine summaries with metric labels
    duration_summary.insert(0, 'Metric', 'Duration')
    touches_summary.insert(0, 'Metric', 'Touches')
    
    combined_summary = pd.concat([duration_summary, touches_summary], axis=0)
    combined_summary.insert(0, 'Predictors', combined_summary.index)
    
    rows = combined_summary[["Metric", "Predictors", "mean", "CI", "ess_bulk"]].astype(str).values.tolist()
    
    header = ["Metric", "Predictors", "Estimates", "CI (2.5%, 97.5%)", "ESS"]
    
    table = Table(
        title="Combined hierarchical regression model predictions per condition",
        header=header,
        rows=rows
    )
    
    # Rename elements for both metrics
    for metric in ["Duration", "Touches"]:
        table.rename_element(f"{metric}_0.0", f"{metric} <0.01")
        table.rename_element(f"{metric}_Intercept", f"{metric} Participant-specific intercept αj")
        table.rename_element(f"{metric}_condition_idx[0]", f"{metric} InSitu")
        table.rename_element(f"{metric}_condition_idx[1]", f"{metric} Interaction")
        table.rename_element(f"{metric}_condition_idx[2]", f"{metric} NoInteraction")
        table.rename_element(f"{metric}_condition_idx[3]", f"{metric} NoOpponent")
        table.rename_element(f"{metric}_1|participant_sigma", f"{metric} σ Participant")
    
    persistence.save_table(table, file_name) 