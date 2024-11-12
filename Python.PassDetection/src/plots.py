import os

import matplotlib.pyplot as plt

from src.domain import LabeledTrial


def plot_labeled_trial(trial: LabeledTrial, save_dir: str, filename: str):
    # Create the save directory if it doesn't exist
    os.makedirs(save_dir, exist_ok=True)

    # Number of subplots
    num_subplots = 3
    fig, axs = plt.subplots(num_subplots, 1, figsize=(12, num_subplots * 3))
    fig.tight_layout(pad=4.0)

    # Time axis (using timestamps)
    time = trial.timestamps

    # Include Trial Number and Pass Event in the Plot Title
    pass_text = 'Pass' if trial.is_a_pass else 'No Pass'
    fig.suptitle(f'Trial Number: {trial.trial_number} - {pass_text}', fontsize=16)

    # Subplot 1: Dominant Foot Positions
    axs[0].set_title('Dominant Foot Positions')
    axs[0].plot(time, [pos.x for pos in trial.user_dominant_foot_positions], label='x')
    axs[0].plot(time, [pos.y for pos in trial.user_dominant_foot_positions], label='y')
    axs[0].plot(time, [pos.z for pos in trial.user_dominant_foot_positions], label='z')
    axs[0].legend()
    axs[0].set_xlabel('Time (s)')
    axs[0].set_ylabel('Position (units)')
    axs[0].grid(True)

    # Subplot 2: Non-Dominant Foot Positions
    axs[1].set_title('Non-Dominant Foot Positions')
    axs[1].plot(time, [pos.x for pos in trial.user_non_dominant_foot_positions], label='x')
    axs[1].plot(time, [pos.y for pos in trial.user_non_dominant_foot_positions], label='y')
    axs[1].plot(time, [pos.z for pos in trial.user_non_dominant_foot_positions], label='z')
    axs[1].legend()
    axs[1].set_xlabel('Time (s)')
    axs[1].set_ylabel('Position (units)')
    axs[1].grid(True)

    # Subplot 3: Distance Between Feet (offset)
    axs[2].set_title('Distance Between Dominant and Non-Dominant Foot')
    axs[2].plot(
        time,
        [((pos_d.x - pos_nd.x) ** 2 + (pos_d.y - pos_nd.y) ** 2 + (pos_d.z - pos_nd.z) ** 2) ** 0.5
         for pos_d, pos_nd in zip(trial.user_dominant_foot_positions, trial.user_non_dominant_foot_positions)],
        label='Distance'
    )
    axs[2].legend()
    axs[2].set_xlabel('Time (s)')
    axs[2].set_ylabel('Distance (units)')
    axs[2].grid(True)

    # Draw Vertical Line at Pass Event Time
    if trial.is_a_pass and trial.pass_id is not None:
        pass_time = trial.timestamps[trial.pass_id]
        for ax in axs:
            ax.axvline(
                x=pass_time,
                color='green',
                linestyle='--',
                linewidth=2,
                label='Pass Event'
            )
        # Update legends to include the pass event line
        for ax in axs:
            handles, labels = ax.get_legend_handles_labels()
            if 'Pass Event' not in labels:
                ax.legend(handles, labels)

    # Adjust layout and add main title
    plt.tight_layout(rect=[0, 0.03, 1, 0.95])  # Leave space for suptitle

    # Save the plot
    plot_path = os.path.join(save_dir, filename)
    plt.savefig(plot_path)
    plt.close(fig)
