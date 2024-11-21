import os
import pickle
from typing import List

from matplotlib import pyplot as plt

from src.domain.inferences import Split
from src.domain.samples import Sample
from src.services.plotter import plot_sample_with_features

with open('dataset.pkl', 'rb') as f:
    samples: List[Sample] = pickle.load(f)

plot_dir = 'plots'
os.makedirs(plot_dir, exist_ok=True)

for idx, sample in enumerate(samples):

    if sample.pass_event.is_a_pass:
        print(sample.pass_event.is_a_pass)

    if sample.inference.pass_probability < 0.9:
        continue

    if sample.augmentation.rotation_angle != 0:
        continue

    if sample.augmentation.swapped_feet:
        continue

    if sample.inference.split == Split.TRAIN:
        continue

    filename = f"Sample_{sample.recording.trial_number}_{idx}_Pass_{sample.pass_event.is_a_pass}.png"
    fig = plot_sample_with_features(sample)
    plot_path = os.path.join(plot_dir, filename)
    fig.savefig(plot_path)
    plt.show()
    plt.close(fig)
