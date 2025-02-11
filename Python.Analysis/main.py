import glob
import re

from src.domain import Condition
from src.manual_annotations import ingest
from src.persistence import CSVPersistence

import matplotlib.pyplot as plt
import seaborn as sns
import pandas as pd
import arviz as az


# Define the path to your data files
data_path = "../Data/Experiment/**/*.csv"
persistence = CSVPersistence()

files = glob.glob(data_path, recursive=True)
trials = []
for csv_file in files:
    json_file = csv_file.replace(".csv", ".json")
    trial = ingest(csv_file, json_file)
    trials.append(trial)
    trial.accept(persistence)

persistence.save(trials, "results.csv")


conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
# Function to convert condition enum to formatted string
def format_condition(condition):
    condition_str = condition.value  # Get the string representation from Enum
    return " ".join([word.capitalize() for word in re.sub(r'([a-z])([A-Z])', r'\1 \2', condition_str).split()])


# Variables to plot
metrics = [
    "number_of_touches",
    "duration",
    "timing_between_last_touch_and_pass",
    # "distance_between_last_touch_and_pass",
    # "average_interpersonal_distance",
    # "interpersonal_distance_at_pass_time"
]

# Define colors
blue = "#4A90E2"
red = "#8B0000"





"""Violin plots"""
# Aggregate data: compute mean per participant per condition for each metric
aggregated_data = []
for condition in conditions:
    participant_data = {metric: {} for metric in metrics}
    for trial in trials:
        if trial.condition == condition:
            for metric in metrics:
                value = getattr(trial, metric)()
                if trial.participant_id not in participant_data[metric]:
                    participant_data[metric][trial.participant_id] = []
                participant_data[metric][trial.participant_id].append(value)

    for metric in metrics:
        for participant, values in participant_data[metric].items():
            aggregated_data.append({
                'Participant': participant,
                'Condition': format_condition(condition),
                'Metric': metric,
                'Value': sum(values) / len(values)
            })

df = pd.DataFrame(aggregated_data)

# Create subplots for each metric
fig, axes = plt.subplots(len(metrics), 1, figsize=(10, 3.5 * len(metrics)), sharex=True)

for ax, metric in zip(axes, metrics):
    sub_df = df[df["Metric"] == metric]

    # Violin plot
    sns.violinplot(
        data=sub_df,
        color=red,
        x="Condition",
        y="Value",
        inner="quartile",
        scale="width",
        ax=ax
    )

    ax.set_title(metric.replace("_", " ").capitalize())

plt.xticks()
plt.tight_layout()
plt.show()


"""Box plot"""
# per condition
distances = []
for condition in conditions:
    distances.append([trial.number_of_touches() for trial in trials if trial.condition == condition])

plt.boxplot(distances, tick_labels=[str(condition) for condition in conditions])
plt.show()



"""Bar plots"""
# Aggregate data: compute mean per participant per condition for each metric
aggregated_data = []
for condition in conditions:
    participant_data = {metric: {} for metric in metrics}
    for trial in trials:
        if trial.condition == condition:
            for metric in metrics:
                value = getattr(trial, metric)()  # Call the function dynamically
                if trial.participant_id not in participant_data[metric]:
                    participant_data[metric][trial.participant_id] = []
                participant_data[metric][trial.participant_id].append(value)

    for metric in metrics:
        for participant, values in participant_data[metric].items():
            aggregated_data.append({
                'Participant': participant,
                'Condition': format_condition(condition),
                'Metric': metric,
                'Value': sum(values) / len(values)  # Average per participant
            })

df = pd.DataFrame(aggregated_data)

# Create subplots for each metric
fig, axes = plt.subplots(len(metrics), 1, figsize=(10, 3.5 * len(metrics)), sharex=True)

for ax, metric in zip(axes, metrics):
    sub_df = df[df["Metric"] == metric]

    # Bar plot (red)
    sns.barplot(
        data=sub_df,
        x="Condition",
        y="Value",
        color=red,
        errorbar=None,
        ax=ax
    )

    # Connect points per participant across conditions
    for participant in sub_df["Participant"].unique():
        participant_data = sub_df[sub_df["Participant"] == participant]
        ax.plot(participant_data["Condition"], participant_data["Value"], marker="o", linestyle="-", color=blue,
                alpha=0.8)

    ax.set_title(metric.replace("_", " ").capitalize())

plt.xticks()
plt.tight_layout()
plt.show()
