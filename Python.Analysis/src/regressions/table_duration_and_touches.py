from pathlib import Path
import pandas as pd
import arviz as az

from src.persistence.persistence import Persistence
from src.persistence.apa_word_table_formatter import Table


def probability_effect_is_zero(x):
    prob = max((x > 0).mean(), (x < 0).mean())
    rounded_prob = round(prob, 2)
    return 1 - rounded_prob


def duration_and_touches_table(duration_model_path: Path, touches_model_path: Path, file_name: Path, persistence: Persistence) -> None:
    duration_results = az.from_netcdf(str(duration_model_path))
    touches_results = az.from_netcdf(str(touches_model_path))
    
    duration_summary = az.summary(
        duration_results,
        hdi_prob=0.95,
        stat_funcs={"Null effect probability": probability_effect_is_zero},
        var_names=["~^mu"],
        filter_vars="regex"
    )
    
    touches_summary = az.summary(
        touches_results,
        hdi_prob=0.95,
        stat_funcs={"Null effect probability": probability_effect_is_zero},
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
        summary["Estimates"] = summary["mean"].apply(lambda x: f"{x:.2f}")

    
    duration_summary.insert(0, 'Metric', 'Trial duration')
    touches_summary.insert(0, 'Metric', 'Number of touches')
    
    combined_summary = pd.concat([duration_summary, touches_summary], axis=0)
    combined_summary.insert(0, 'Predictors', combined_summary.index)
    
    rows = combined_summary[["Metric", "Predictors", "Estimates", "CI", "ess_bulk", "Null effect probability"]].astype(str).values.tolist()

    header = ["Metric", "Predictors", "Estimates", "CI (2.5%, 97.5%)", "ESS", "Null effect probability"]
    
    table = Table(
        title="Combined hierarchical regression model predictions per condition",
        header=header,
        rows=rows
    )
    
    # Rename elements for both metrics
    for metric in ["Duration", "Touches"]:
        table.rename_element("0.0", "<0.01")
        table.rename_element("C(condition)[NoOpponent]", "Interaction[No-opponent]")
        table.rename_element("C(condition)[NoInteraction]", "Interaction[Non-interactive]")
        table.rename_element("C(condition)[Interaction]", "Interaction[Interactive]")
        table.rename_element("C(condition)[InSitu]", "Interaction[In situ]")
        table.rename_element("1|participant_id_sigma", "σj Participant")
        table.rename_element("sigma", "σ")
    
    persistence.save_table(table, file_name)
