from matplotlib import pyplot as plt

from src.domain import SampleWithFeatures, Sample


def plot_sample_with_features(sample: SampleWithFeatures) -> plt.Figure:
    num_features = len(sample.features)
    fig_height = num_features * 2  # Smaller figure height per feature
    fig, axs = plt.subplots(
        num_features, 1, figsize=(10, fig_height),
        gridspec_kw={'hspace': 2}  # Increase vertical space between subplots
    )

    start_time = round(sample.timestamps[0], 1) if sample.timestamps else 0.0
    end_time = round(sample.timestamps[-1], 1) if sample.timestamps else 0.0

    # Pass probability
    prediction = f"Prediction: {sample.pass_probability:.2f}" if sample.pass_probability is not None else "Prediction: N/A"

    # Main title
    pass_text = 'Pass' if sample.is_a_pass else 'No Pass'
    fig.suptitle(
        f"Split: {sample.split.name.lower()} Trial Number: {sample.trial_number} - {pass_text} \n"
        f"Start: {start_time}s, End: {end_time}s, {prediction}",
        fontsize=16
    )

    if num_features == 1:
        axs = [axs]

    for i in range(num_features):
        ax = axs[i]
        feature = sample.features[i]
        time = sample.timestamps
        ax.set_title(feature.name)
        ax.plot(time, feature.values, label=feature.name)
        ax.legend()
        ax.set_xlabel('Time (s)')
        ax.set_ylabel('Value')
        ax.grid(True)

        # If there is a pass event, add vertical line
        if sample.is_a_pass and sample.pass_id is not None:
            ax.axvline(
                x=sample.pass_timestamp,
                color='green',
                linestyle='--',
                linewidth=2,
                label='Pass Event'
            )
            # Update legend to include the pass event line
            handles, labels = ax.get_legend_handles_labels()
            if 'Pass Event' not in labels:
                ax.legend(handles, labels)

    return fig


def plot_sample(sample: Sample):
    dominant_x = [pos.x for pos in sample.user_dominant_foot_positions]
    dominant_y = [pos.y for pos in sample.user_dominant_foot_positions]
    dominant_z = [pos.z for pos in sample.user_dominant_foot_positions]

    non_dominant_x = [pos.x for pos in sample.user_non_dominant_foot_positions]
    non_dominant_y = [pos.y for pos in sample.user_non_dominant_foot_positions]
    non_dominant_z = [pos.z for pos in sample.user_non_dominant_foot_positions]

    timestamps = sample.timestamps

    plt.figure(figsize=(12, 8))
    plt.subplot(3, 1, 1)
    plt.plot(timestamps, dominant_x, label="Dominant Foot X")
    plt.plot(timestamps, non_dominant_x, label="Non-Dominant Foot X")
    plt.ylabel("X Position")
    plt.legend()

    plt.subplot(3, 1, 2)
    plt.plot(timestamps, dominant_y, label="Dominant Foot Y")
    plt.plot(timestamps, non_dominant_y, label="Non-Dominant Foot Y")
    plt.ylabel("Y Position")
    plt.legend()

    plt.subplot(3, 1, 3)
    plt.plot(timestamps, dominant_z, label="Dominant Foot Z")
    plt.plot(timestamps, non_dominant_z, label="Non-Dominant Foot Z")
    plt.xlabel("Time (s)")
    plt.ylabel("Z Position")
    plt.legend()

    plt.tight_layout()
    plt.show()