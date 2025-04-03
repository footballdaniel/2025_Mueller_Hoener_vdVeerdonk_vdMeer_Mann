import glob
from pathlib import Path
import re
import matplotlib.pyplot as plt
import pandas as pd
import seaborn as sns

from src.persistence import ApaStyledPersistence
from src.reader import TrialReader
from src.regression import (
    regression_duration, 
    regression_touches, 
    duration_and_touches_figure, 
    table_duration_and_touches,
    duration_and_touches_figure_post_hoc
)


if __name__ == '__main__':
    data_path = "../Data/Experiment/**/*.csv"

    persistence = ApaStyledPersistence(
        font=Path("Calibri.ttf"),
        font_size=11,
        double_column_width_inches=6,
        single_column_width_inches=3,
        grayscale=False
    )

    trials = TrialReader.read_trials(data_path)

    regression_duration(trials, Path("model_1.nc"), Path("model_1.txt"), persistence)
    regression_touches(trials, Path("model_2.nc"), Path("model_2.txt"), persistence)

    table_duration_and_touches(Path("model_1.nc"), Path("model_2.nc"), Path("model_predictions.docx"), persistence)
    duration_and_touches_figure(Path("model_1.nc"), Path("model_2.nc"), Path("predictions.png"), persistence)
    duration_and_touches_figure_post_hoc(Path("model_1.nc"), Path("model_2.nc"), Path("ridge_differences.png"), persistence)



    # TrialVisualizer.print_trial_quality_summary(trials.trials)
    # TrialVisualizer.analyze_and_plot_all_trials(trials.trials)
    # print(f"\nSaved quality plots to output/quality_plots/")


    # conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]

    # def format_condition(condition):
    #     condition_str = condition.value
    #     return " ".join([word.capitalize() for word in re.sub(r'([a-z])([A-Z])', r'\1 \2', condition_str).split()])

    # metrics = [
    #     "number_of_touches",
    #     "duration",
    #     "timing_between_last_touch_and_pass",
    # ]

    # # Define colors
    # blue = "#4A90E2"
    # red = "#8B0000"

    # aggregated_data = []
    # for condition in conditions:
    #     participant_data = {metric: {} for metric in metrics}  # Moved inside loop
    #     for trial in trials.trials:
    #         if trial.condition == condition:
    #             for metric in metrics:
    #                 value = getattr(trial, metric)()
    #                 if trial.participant_id not in participant_data[metric]:
    #                     participant_data[metric][trial.participant_id] = []
    #                 participant_data[metric][trial.participant_id].append(value)

    #         for participant, values in participant_data[metric].items():
    #             aggregated_data.append({
    #                 'Participant': participant,
    #                 'Condition': format_condition(condition),
    #                 'Metric': metric,
    #                 'Value': sum(values) / len(values)
    #             })

    # df = pd.DataFrame(aggregated_data)  # Keep all aggregated data in df

    # """Only comparable conds"""
    # # Filter data to only include INTERACTION and NO_INTERACTION conditions
    # filtered_conditions = [format_condition(Condition.INTERACTION), format_condition(Condition.NO_INTERACTION)]
    # df_filtered = df[df["Condition"].isin(filtered_conditions)]

    # # Create a single bar plot for the filtered conditions
    # fig, axes = plt.subplots(len(metrics), 1, figsize=(10, 3.5 * len(metrics)), sharex=True)

    # for ax, metric in zip(axes, metrics):
    #     sub_df = df_filtered[df_filtered["Metric"] == metric]

    #     # Bar plot (red)
    #     sns.barplot(
    #         data=sub_df,
    #         x="Condition",
    #         y="Value",
    #         color=red,
    #         errorbar=None,
    #         ax=ax
    #     )

    #     # Connect points per participant across conditions
    #     for participant in sub_df["Participant"].unique():
    #         participant_data = sub_df[sub_df["Participant"] == participant]
    #         ax.plot(participant_data["Condition"], participant_data["Value"], marker="o", linestyle="-", color=blue, alpha=0.8)

    #     ax.set_title(metric.replace("_", " ").capitalize())

    # plt.xticks()
    # plt.tight_layout()
    # plt.show()

    # """Violin plots"""
    # fig, axes = plt.subplots(len(metrics), 1, figsize=(10, 3.5 * len(metrics)), sharex=True)

    # for ax, metric in zip(axes, metrics):
    #     sub_df = df[df["Metric"] == metric]

    #     sns.violinplot(
    #         data=sub_df,
    #         x="Condition",
    #         y="Value",
    #         inner="quartile",
    #         scale="width",
    #         color=red,
    #         ax=ax
    #     )

    #     ax.set_title(metric.replace("_", " ").capitalize())

    # plt.xticks()
    # plt.tight_layout()
    # plt.show()

    # """Box plot"""
    # # per condition
    # distances = []
    # for condition in conditions:
    #     distances.append([trial.number_of_touches() for trial in trials.trials if trial.condition == condition])

    # plt.boxplot(distances, labels=[format_condition(condition) for condition in conditions])
    # plt.show()

    # """Bar plots"""
    # # Aggregate data again for bar plots to avoid overwriting previous aggregation
    # aggregated_data_bar = []
    # for condition in conditions:
    #     participant_data = {metric: {} for metric in metrics}
    #     for trial in trials.trials:
    #         if trial.condition == condition:
    #             for metric in metrics:
    #                 value = getattr(trial, metric)()
    #                 if trial.participant_id not in participant_data[metric]:
    #                     participant_data[metric][trial.participant_id] = []
    #                 participant_data[metric][trial.participant_id].append(value)

    #     for metric in metrics:
    #         for participant, values in participant_data[metric].items():
    #             aggregated_data_bar.append({
    #                 'Participant': participant,
    #                 'Condition': format_condition(condition),
    #                 'Metric': metric,
    #                 'Value': sum(values) / len(values)
    #             })

    # df_bar = pd.DataFrame(aggregated_data_bar)

    # # Create subplots for each metric
    # fig, axes = plt.subplots(len(metrics), 1, figsize=(10, 3.5 * len(metrics)), sharex=True)

    # for ax, metric in zip(axes, metrics):
    #     sub_df = df_bar[df_bar["Metric"] == metric]

    #     sns.barplot(
    #         data=sub_df,
    #         x="Condition",
    #         y="Value",
    #         color=red,
    #         errorbar=None,
    #         ax=ax
    #     )

    #     for participant in sub_df["Participant"].unique():
    #         participant_data = sub_df[sub_df["Participant"] == participant]
    #         ax.plot(participant_data["Condition"], participant_data["Value"], marker="o", linestyle="-", color=blue, alpha=0.8)

    #     ax.set_title(metric.replace("_", " ").capitalize())

    # plt.xticks()
    # plt.tight_layout()
    # plt.show()
