import os
from pathlib import Path
from typing import List

from matplotlib import pyplot as plt

from src.domain.samples import Sample


class TimeSeriesPlot:
    def __init__(self, plot_dir: Path):
        self.sample = None
        self.plot_dir = plot_dir
        self.fig, self.axs = None, None
        self.num_features = 0
        self.features = []

    def add_sample(self, sample: Sample):
        self.sample = sample

    def add_feature(self, feature_name: str, feature_values: List[float]):
        self.features.append((feature_name, feature_values))

    def save(self):
        if not self.plot_dir.exists():
            self.plot_dir.mkdir(parents=True, exist_ok=True)
        plot_path = os.path.join(str(self.plot_dir), f"{self.sample.id}_pass_{self.sample.contains_a_pass() == 1}.png")

        num_features = len(self.features)
        fig_height = num_features * 2
        fig, axs = plt.subplots(num_features, 1, figsize=(10, fig_height), gridspec_kw={'hspace': 2})
        if num_features == 1:
            axs = [axs]

        for i, (feature_name, feature_values) in enumerate(self.features):
            ax = axs[i]
            ax.set_title(feature_name)
            ax.plot(self.sample.recording.timestamps, feature_values, label=feature_name)
            ax.legend()
            ax.set_xlabel('Time (s)')
            ax.set_ylabel('Value')
            ax.grid(True)

            # Plot vertical line if an event has happened
            for event in self.sample.recording.events:
                if event.is_pass:
                    ax.axvline(x=event.timestamp, color='green', linestyle='--', linewidth=2, label='Pass Event')
                    handles, labels = ax.get_legend_handles_labels()
                    if 'Pass Event' not in labels:
                        ax.legend(handles, labels)

        # Add a main title to the figure
        fig.suptitle(f"Sample ID: {self.sample.id} - Pass: {self.sample.contains_a_pass() == 1}", fontsize=16)
        fig.savefig(plot_path)
        plt.close(fig)
