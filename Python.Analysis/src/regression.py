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


def regression_1(trials: TrialCollection, model_path: Path, persistence: Persistence) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials.trials],
        "duration": [trial.duration() for trial in trials.trials],
        "condition": [trial.condition.value for trial in trials.trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    df["condition_idx"] = pd.Categorical(df["condition"]).codes
    
    flat_prior = bmb.Prior("Uniform", lower=-3, upper=3)
    priors = {
        "C(condition)": flat_prior
    }
    formula = "duration ~ 0 + C(condition) + (1|participant_id)"
    
    model_bambi = bmb.Model(
        formula,
        df,
        family="gaussian",
        priors=priors,
    )
    
    # persistence.save_model(model_bambi.__str__(), Path("model_1.txt"))
    
    if not os.path.exists(str(model_path)):
        results = model_bambi.fit(
            draws=100,
            chains=4,
            idata_kwargs={"log_likelihood": True},
            random_seed=1991
        )
        
        model_bambi.predict(results, kind="response")
        
        az.to_netcdf(results, str(model_path))


def predictive_figure_1(inference_data: Path, file_name: Path, persistence: Persistence) -> None:
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


def table_1(inference_data: Path, file_name: Path, persistence: Persistence) -> None:
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