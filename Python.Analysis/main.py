import glob

from src.manual_annotations import process_file

# Define the path to your data files
data_path = "../Data/Experiment/**/*.csv"

files = glob.glob(data_path, recursive=True)

trials = []
for csv_file in files:
    print(f"Processing file: {csv_file}")
    json_file = csv_file.replace(".csv", ".json")
    trials.append(process_file(csv_file, json_file))
