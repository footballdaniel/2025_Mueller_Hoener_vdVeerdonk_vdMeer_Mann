import os

import matplotlib.pyplot as plt

from src.features import PositionAndVelocityFeature


def plot_feature(feature: PositionAndVelocityFeature, save_dir: str, filename: str):
    # Create the save directory if it doesn't exist
    os.makedirs(save_dir, exist_ok=True)

    # Number of subplots
    num_subplots = 4
    fig, axs = plt.subplots(num_subplots, 1, figsize=(12, num_subplots * 3))
    fig.tight_layout(pad=4.0)

    # Time axis (using timestamps)
    time = feature.timestamps

    # Include Trial Number and Pass Event in the Plot Title
    pass_text = 'Pass' if feature.is_a_pass else 'No Pass'
    fig.suptitle(f'Trial Number: {feature.trial_number} - {pass_text}', fontsize=16)

    # Subplot 1: Zeroed Positions
    axs[0].set_title('Zeroed Position Dominant Foot')
    axs[0].plot(time, [pos.x for pos in feature.zeroed_position_dominant_foot], label='x')
    axs[0].plot(time, [pos.y for pos in feature.zeroed_position_dominant_foot], label='y')
    axs[0].plot(time, [pos.z for pos in feature.zeroed_position_dominant_foot], label='z')
    axs[0].legend()
    axs[0].set_xlabel('Time (s)')
    axs[0].set_ylabel('Position (units)')
    axs[0].grid(True)

    # Subplot 2: Offsets
    axs[1].set_title('Offset Dominant to Non-Dominant Foot')
    axs[1].plot(time, [pos.x for pos in feature.offset_dominant_foot_to_non_dominant_foot], label='x')
    axs[1].plot(time, [pos.y for pos in feature.offset_dominant_foot_to_non_dominant_foot], label='y')
    axs[1].plot(time, [pos.z for pos in feature.offset_dominant_foot_to_non_dominant_foot], label='z')
    axs[1].legend()
    axs[1].set_xlabel('Time (s)')
    axs[1].set_ylabel('Offset (units)')
    axs[1].grid(True)

    # Subplot 3: Velocities Dominant Foot
    axs[2].set_title('Velocities Dominant Foot')
    axs[2].plot(time, [vel.x for vel in feature.velocities_dominant_foot], label='x')
    axs[2].plot(time, [vel.y for vel in feature.velocities_dominant_foot], label='y')
    axs[2].plot(time, [vel.z for vel in feature.velocities_dominant_foot], label='z')
    axs[2].legend()
    axs[2].set_xlabel('Time (s)')
    axs[2].set_ylabel('Velocity (units/s)')
    axs[2].grid(True)

    # Subplot 4: Velocities Non-Dominant Foot
    axs[3].set_title('Velocities Non-Dominant Foot')
    axs[3].plot(time, [vel.x for vel in feature.velocities_non_dominant_foot], label='x')
    axs[3].plot(time, [vel.y for vel in feature.velocities_non_dominant_foot], label='y')
    axs[3].plot(time, [vel.z for vel in feature.velocities_non_dominant_foot], label='z')
    axs[3].legend()
    axs[3].set_xlabel('Time (s)')
    axs[3].set_ylabel('Velocity (units/s)')
    axs[3].grid(True)

    # Draw Vertical Line at Pass Event Time
    if feature.is_a_pass and feature.pass_event_timestamp is not None:
        for ax in axs:
            ax.axvline(
                x=feature.pass_event_timestamp,
                color='green',
                linestyle='--',
                linewidth=6,
                label='Pass Event'
            )
        # Update legends to include the pass event line
        for ax in axs:
            handles, labels = ax.get_legend_handles_labels()
            # Avoid duplicate labels
            if 'Pass Event' not in labels:
                ax.legend(handles, labels)
            else:
                ax.legend()

    # Adjust layout and add main title
    plt.tight_layout(rect=[0, 0.03, 1, 0.95])  # Leave space for suptitle

    # Save the plot
    plot_path = os.path.join(save_dir, filename)
    plt.savefig(plot_path)
    plt.close(fig)
