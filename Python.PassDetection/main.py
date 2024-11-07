import glob
import os

from src.io import read_pass_events_from_csv, read_trial_from_json
from src.plots import plot_feature
from src.training_data_sampler import DataSampler

# Use glob to find all CSV files
pattern = "data/**/*.csv"

trials = []
for filename in glob.iglob(pattern, recursive=True):
    print(f"Processing file: {filename}")
    json_filename = filename.replace(".csv", ".json")
    if not os.path.isfile(json_filename):
        print("No JSON file found for", filename)
        continue

    trial = read_trial_from_json(json_filename)
    pass_events = read_pass_events_from_csv(filename)
    trial.pass_events.extend(pass_events)  # Append the events to the Trial

    print("Trial instance created successfully:", trial)
    trials.append(trial)

sampler = DataSampler(trials)
full_dataset = sampler.generate_dataset()
print(full_dataset[0])

"""AUGMENTATION"""
rotation_angles = [angle for angle in range(5, 360, 10)]
augmented_passes = []
for feature in full_dataset:
    augmented_passes.append(feature)  # Include the original feature

    if feature.is_a_pass:
        for angle in rotation_angles:
            rotated_feature = feature.rotate_around_y(angle)
            augmented_passes.append(rotated_feature)

        swapped_feature = feature.swap_feet()
        augmented_passes.append(swapped_feature)

        for angle in rotation_angles:
            swapped_rotated_feature = swapped_feature.rotate_around_y(angle)
            augmented_passes.append(swapped_rotated_feature)


combined_dataset = full_dataset + augmented_passes

for sample in full_dataset:
    plot_feature(sample, 'plots', f'{sample.trial_number}_{sample.start_time}_{sample.is_a_pass}.png')
