import glob

from matplotlib import pyplot as plt

from src.domain import Condition
from src.manual_annotations import process_file

# Define the path to your data files
data_path = "../Data/Experiment/**/*.csv"

files = glob.glob(data_path, recursive=True)

trials = []
for csv_file in files:
    print(f"Processing file: {csv_file}")
    json_file = csv_file.replace(".csv", ".json")
    trials.append(process_file(csv_file, json_file))


# calculate average duration of trial per condition
def calculate_average_duration(trials, condition):
    durations = []
    for trial in trials:
        if trial.condition == condition:
            durations.append(trial.duration())
    return sum(durations) / len(durations)


def calculate_average_number_of_touches(trials, condition):
    touches = []
    for trial in trials:
        if trial.condition == condition:
            touches.append(trial.number_of_touches())
    return sum(touches) / len(touches)


def calculate_average_ball_travel_length(trials, condition):
    lengths = []
    for trial in trials:
        if trial.condition == condition:
            lengths.append(trial.travel_distance())
    return sum(lengths) / len(lengths)


# print
print(f"Average duration of InSitu trials: {calculate_average_duration(trials, Condition.IN_SITU)}")
print(f"Average duration of Interaction trials: {calculate_average_duration(trials, Condition.INTERACTION)}")
print(f"Average duration of NoInteraction trials: {calculate_average_duration(trials, Condition.NO_INTERACTION)}")
print(f"Average duration of NoOpponent trials: {calculate_average_duration(trials, Condition.NO_OPPONENT)}")

print(f"Average number of touches of InSitu trials: {calculate_average_number_of_touches(trials, Condition.IN_SITU)}")
print(
    f"Average number of touches of Interaction trials: {calculate_average_number_of_touches(trials, Condition.INTERACTION)}")
print(
    f"Average number of touches of NoInteraction trials: {calculate_average_number_of_touches(trials, Condition.NO_INTERACTION)}")
print(
    f"Average number of touches of NoOpponent trials: {calculate_average_number_of_touches(trials, Condition.NO_OPPONENT)}")

# Data
conditions = ["InSitu", "Interaction", "NoInteraction", "NoOpponent"]
durations = [
    calculate_average_duration(trials, Condition.IN_SITU),
    calculate_average_duration(trials, Condition.INTERACTION),
    calculate_average_duration(trials, Condition.NO_INTERACTION),
    calculate_average_duration(trials, Condition.NO_OPPONENT),
]
touches = [
    calculate_average_number_of_touches(trials, Condition.IN_SITU),
    calculate_average_number_of_touches(trials, Condition.INTERACTION),
    calculate_average_number_of_touches(trials, Condition.NO_INTERACTION),
    calculate_average_number_of_touches(trials, Condition.NO_OPPONENT),
]

# Plot durations
plt.figure(figsize=(8, 5))
plt.bar(conditions, durations)
plt.xlabel("Condition")
plt.ylabel("Average Duration")
plt.title("Average Duration of Trials")
plt.show()

# Plot number of touches
plt.figure(figsize=(8, 5))
plt.bar(conditions, touches)
plt.xlabel("Condition")
plt.ylabel("Average Number of Touches")
plt.title("Average Number of Touches in Trials")
plt.show()