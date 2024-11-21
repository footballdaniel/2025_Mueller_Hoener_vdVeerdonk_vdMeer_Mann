import os
import pickle

from matplotlib import pyplot as plt

from src.domain import Split
from src.plots import plot_sample_with_features

with open('dataset.pkl', 'rb') as f:
    samples = pickle.load(f)

plot_dir = 'plots'
os.makedirs(plot_dir, exist_ok=True)

for idx, sample in enumerate(samples):

    if sample.pass_probability < 0.9:
        continue

    if sample.rotation_angle != 0:
        continue

    if sample.swapped_feet:
        continue

    if sample.split == Split.TRAIN:
        continue

    filename = f"Sample_{sample.trial_number}_{idx}_Pass_{sample.is_a_pass}.png"
    fig = plot_sample_with_features(sample)
    plot_path = os.path.join(plot_dir, filename)
    fig.savefig(plot_path)
    # plt.show()
    plt.close(fig)

print(f"Plots saved in directory: {plot_dir}")