from typing import List, Dict, Tuple
import matplotlib.pyplot as plt
import numpy as np
import os
from pathlib import Path
from src.domain import Trial, Position, Condition


class TrialVisualizer:
    @staticmethod
    def get_trial_segment(trial: Trial) -> Tuple[List[float], List[Position], List[Position], List[Position], List[Position], List[Position]]:
        start_idx = trial.start.time_index
        end_idx = trial.pass_event.time_index
        
        return (
            trial.timestamps[start_idx:end_idx + 1],
            trial.head_positions[start_idx:end_idx + 1],
            trial.dominant_foot_positions[start_idx:end_idx + 1],
            trial.non_dominant_foot_positions[start_idx:end_idx + 1],
            trial.hip_positions[start_idx:end_idx + 1],
            trial.opponent_hip_positions[start_idx:end_idx + 1]
        )

    @staticmethod
    def has_missing_data(positions: List[Position]) -> bool:
        if not positions:
            return True
        null_flags = TrialVisualizer.detect_null_positions(positions)
        return any(null_flags)

    @staticmethod
    def detect_null_positions(positions: List[Position]) -> List[bool]:
        """
        Detect null positions using the same logic as Interpolator.
        
        Args:
            positions: List of positions to analyze
            
        Returns:
            List of boolean flags indicating null positions
        """
        if not positions:
            return []
            
        null_flags = [False] * len(positions)
        for i in range(len(positions) - 1, 0, -1):
            if positions[i].distance_2d(positions[i - 1]) == 0:
                null_flags[i] = True
        return null_flags

    @staticmethod
    def calculate_null_percentage(positions: List[Position]) -> float:
        """
        Calculate the percentage of null positions in a list.
        
        Args:
            positions: List of positions to analyze
            
        Returns:
            Percentage of null positions (0-100)
        """
        if not positions:
            return 100.0
            
        null_flags = TrialVisualizer.detect_null_positions(positions)
        return (sum(null_flags) / len(null_flags)) * 100

    @staticmethod
    def plot_trial_data_quality(trial: Trial, output_dir: str, ignore_opponent_in_no_opponent: bool = True) -> str:
        """
        Create a plot showing the quality of position data for a single trial and save it to a file.
        
        Args:
            trial: The trial to visualize
            output_dir: Directory to save the plot
            
        Returns:
            Path to the saved plot file
        """
        # Create output directory if it doesn't exist
        os.makedirs(output_dir, exist_ok=True)
        
        timestamps, head_pos, dom_foot_pos, non_dom_foot_pos, hip_pos, opp_hip_pos = TrialVisualizer.get_trial_segment(trial)
        
        # Check if trial has any missing data
        has_missing = any([
            TrialVisualizer.has_missing_data(head_pos),
            TrialVisualizer.has_missing_data(dom_foot_pos),
            TrialVisualizer.has_missing_data(non_dom_foot_pos),
            TrialVisualizer.has_missing_data(hip_pos),
            (not ignore_opponent_in_no_opponent or trial.condition != Condition.NO_OPPONENT) and 
            TrialVisualizer.has_missing_data(opp_hip_pos)
        ])
        
        if not has_missing:
            return ""
        
        # Create figure with subplots for each position type
        fig, axes = plt.subplots(5, 1, figsize=(15, 12), sharex=True)
        fig.suptitle(f'Trial {trial.trial_number} - Participant {trial.participant_id} - {trial.condition.value}')
        
        # Define position types and their labels
        position_types = [
            (head_pos, 'Head'),
            (dom_foot_pos, 'Dominant Foot'),
            (non_dom_foot_pos, 'Non-Dominant Foot'),
            (hip_pos, 'Hip'),
            (opp_hip_pos, 'Opponent Hip')
        ]
        
        # Plot each position type
        for ax, (positions, label) in zip(axes, position_types):
            if not positions:
                ax.text(0.5, 0.5, 'No data available', 
                       horizontalalignment='center',
                       verticalalignment='center',
                       transform=ax.transAxes)
                ax.set_title(f'{label} Position (No Data)')
                continue
                
            null_flags = TrialVisualizer.detect_null_positions(positions)
            
            # Create time array
            times = np.array(timestamps)
            
            # Plot the data
            ax.plot(times, [pos.x for pos in positions], 'b-', label='X', alpha=0.7)
            ax.plot(times, [pos.y for pos in positions], 'g-', label='Y', alpha=0.7)
            ax.plot(times, [pos.z for pos in positions], 'r-', label='Z', alpha=0.7)
            
            # Highlight null regions
            null_regions = []
            start_idx = None
            for i, is_null in enumerate(null_flags):
                if is_null and start_idx is None:
                    start_idx = i
                elif not is_null and start_idx is not None:
                    null_regions.append((start_idx, i - 1))  # Changed to i-1 to avoid out of bounds
                    start_idx = None
            
            # Add the last region if it exists
            if start_idx is not None:
                null_regions.append((start_idx, len(null_flags) - 1))  # Changed to len(null_flags)-1
            
            # Shade null regions
            for start, end in null_regions:
                if start <= end < len(times):  # Added bounds check
                    ax.axvspan(times[start], times[end], color='red', alpha=0.2)
            
            # Customize the subplot
            ax.set_title(f'{label} Position')
            ax.set_ylabel('Position (m)')
            ax.grid(True, alpha=0.3)
            ax.legend()
            
            # Add null percentage to the title
            null_percentage = TrialVisualizer.calculate_null_percentage(positions)
            ax.set_title(f'{label} Position (Null: {null_percentage:.1f}%)')
        
        # Set x-axis label only on the bottom subplot
        axes[-1].set_xlabel('Time (s)')
        
        # Adjust layout
        plt.tight_layout()
        
        # Create filename
        filename = f"trial_{trial.trial_number}_participant_{trial.participant_id}_{trial.condition.value.lower()}.png"
        filepath = os.path.join(output_dir, filename)
        
        # Save the plot
        plt.savefig(filepath, dpi=300, bbox_inches='tight')
        plt.close()
        
        return filepath

    @staticmethod
    def analyze_and_plot_all_trials(trials: List[Trial], output_dir: str = "output/quality_plots", ignore_opponent_in_no_opponent: bool = True) -> List[str]:
        """
        Analyze and plot data quality for all trials, saving plots to files.
        
        Args:
            trials: List of trials to analyze
            output_dir: Directory to save the plots
            
        Returns:
            List of paths to the saved plot files
        """
        saved_files = []
        for trial in trials:
            filepath = TrialVisualizer.plot_trial_data_quality(trial, output_dir, ignore_opponent_in_no_opponent)
            if filepath:  # Only add if a plot was created (trial had missing data)
                saved_files.append(filepath)
        return saved_files

    @staticmethod
    def print_trial_quality_summary(trials: List[Trial], ignore_opponent_in_no_opponent: bool = True) -> None:
        """
        Print a summary of data quality for all trials.
        
        Args:
            trials: List of trials to analyze
        """
        print("\nTrial Data Quality Summary (Start to Pass):")
        print("-" * 80)
        print(f"{'Trial':<8} {'Participant':<10} {'Condition':<15} {'Head':<8} {'Dom Foot':<10} "
              f"{'Non-Dom Foot':<12} {'Hip':<8} {'Opp Hip':<10}")
        print("-" * 80)
        
        for trial in trials:
            _, head_pos, dom_foot_pos, non_dom_foot_pos, hip_pos, opp_hip_pos = TrialVisualizer.get_trial_segment(trial)
            
            head_null = TrialVisualizer.calculate_null_percentage(head_pos)
            dom_foot_null = TrialVisualizer.calculate_null_percentage(dom_foot_pos)
            non_dom_foot_null = TrialVisualizer.calculate_null_percentage(non_dom_foot_pos)
            hip_null = TrialVisualizer.calculate_null_percentage(hip_pos)
            opp_hip_null = (TrialVisualizer.calculate_null_percentage(opp_hip_pos) 
                          if not ignore_opponent_in_no_opponent or trial.condition != Condition.NO_OPPONENT 
                          else 0.0)
            
            print(f"{trial.trial_number:<8} {trial.participant_id:<10} {trial.condition.value:<15} "
                  f"{head_null:>6.1f}% {dom_foot_null:>8.1f}% {non_dom_foot_null:>10.1f}% "
                  f"{hip_null:>6.1f}% {opp_hip_null:>8.1f}%")
        
        print("-" * 80) 