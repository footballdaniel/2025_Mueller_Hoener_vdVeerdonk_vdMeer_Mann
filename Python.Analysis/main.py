import glob
import re
import matplotlib.pyplot as plt
import math
import pandas as pd
import seaborn as sns
from statsmodels.stats.power import TTestIndPower
from src.domain import Condition
from src.manual_annotations import ingest
from src.persistence import CSVPersistence

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
    condition_str = condition.value
    return " ".join([word.capitalize() for word in re.sub(r'([a-z])([A-Z])', r'\1 \2', condition_str).split()])

# Variables to plot
metrics = [
    "number_of_touches",
    "duration",
    "timing_between_last_touch_and_pass",
]

# Define colors
blue = "#4A90E2"
red = "#8B0000"

# Aggregate data: compute mean per participant per condition for each metric
aggregated_data = []
selected_trials = [trial for trial in trials if trial.participant_id in {1, 2, 3, 4, 5, 6}]
for condition in conditions:
    participant_data = {metric: {} for metric in metrics}
    for trial in selected_trials:
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

df = pd.DataFrame(aggregated_data)  # Keep all aggregated data in df

"""Only comparable conds"""
# Filter data to only include INTERACTION and NO_INTERACTION conditions
filtered_conditions = [format_condition(Condition.INTERACTION), format_condition(Condition.NO_INTERACTION)]
df_filtered = df[df["Condition"].isin(filtered_conditions)]
participants_in_dataset = len({trial.participant_id for trial in selected_trials})
print(f"Participants in dataset: {participants_in_dataset}")
print("Effect size uses Cohen's d; unitless, in pooled SD units.")
power_analysis = TTestIndPower()
power_results = []
for metric in metrics:
    sub_df = df_filtered[df_filtered["Metric"] == metric]
    interaction_values = sub_df[sub_df["Condition"] == format_condition(Condition.INTERACTION)]["Value"]
    no_interaction_values = sub_df[sub_df["Condition"] == format_condition(Condition.NO_INTERACTION)]["Value"]
    mean1 = interaction_values.mean()
    mean2 = no_interaction_values.mean()
    std1 = interaction_values.std()
    std2 = no_interaction_values.std()
    n1 = len(interaction_values)
    n2 = len(no_interaction_values)
    pooled_sd = (((n1 - 1) * (std1 ** 2) + (n2 - 1) * (std2 ** 2)) / (n1 + n2 - 2)) ** 0.5
    pooled_variance = pooled_sd ** 2
    effect_size = (mean1 - mean2) / pooled_sd
    required_per_condition = math.ceil(power_analysis.solve_power(effect_size=abs(effect_size), alpha=0.05, power=0.8, alternative="two-sided"))
    power_results.append((metric, effect_size, required_per_condition, pooled_variance))
for metric, effect_size, required_per_condition, pooled_variance in power_results:
    print(f"{metric}: pooled_variance={pooled_variance:.4f}, effect_size={effect_size:.3f}, participants_per_condition_needed={required_per_condition}, total_needed={required_per_condition * 2}")
