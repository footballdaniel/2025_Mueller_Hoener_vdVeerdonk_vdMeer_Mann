import glob

from src.manual_annotations import process_file

# Define the path to your data files
data_path = "../Data/Pilot_2/*.csv"

# Process all CSV files in the specified directory
files = glob.glob(data_path)
for csv_file in files:
    print(f"Processing file: {csv_file}")
    json_file = csv_file.replace(".csv", ".json")
    process_file(csv_file, json_file)
