import os
import pickle

from matplotlib import pyplot as plt

from src.plots import plot_sample_with_features

# Load the dataset from the pickle file
with open('dataset.pkl', 'rb') as f:
    samples = pickle.load(f)

# Ensure output directory exists
plot_dir = 'plots'
os.makedirs(plot_dir, exist_ok=True)

# Iterate over samples and plot each one
for idx, sample in enumerate(samples):

    # if sample.pass_probability < 0.5:
    #     continue

    filename = f"sample_{sample.trial_number}_{idx}_pass{sample.is_a_pass}.png"
    fig = plot_sample_with_features(sample)
    plot_path = os.path.join(plot_dir, filename)
    fig.savefig(plot_path)
    # plt.show()
    plt.close(fig)

print(f"Plots saved in directory: {plot_dir}")