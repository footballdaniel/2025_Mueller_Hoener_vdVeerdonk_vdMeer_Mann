import glob

from matplotlib import pyplot as plt

from src.domain import Condition
from src.manual_annotations import ingest

# Define the path to your data files
data_path = "../Data/Experiment/**/*.csv"

files = glob.glob(data_path, recursive=True)

trials = []
for csv_file in files:
    json_file = csv_file.replace(".csv", ".json")
    trial = ingest(csv_file, json_file)
    trials.append(trial)

# per condition
conditions = [Condition.IN_SITU, Condition.INTERACTION, Condition.NO_INTERACTION, Condition.NO_OPPONENT]
distances = []
for condition in conditions:
    distances.append([trial.number_of_touches() for trial in trials if trial.condition == condition])

plt.boxplot(distances, tick_labels=[str(condition) for condition in conditions])
plt.show()

