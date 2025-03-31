from typing import List, Dict, Tuple
import matplotlib.pyplot as plt
import numpy as np
import os
from pathlib import Path
from src.domain import Trial, Position


class TrialVisualizer:
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
    def plot_trial_data_quality(trial: Trial, output_dir: str) -> str:
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
        
        # Create figure with subplots for each position type
        fig, axes = plt.subplots(5, 1, figsize=(15, 12), sharex=True)
        fig.suptitle(f'Trial {trial.trial_number} - Participant {trial.participant_id} - {trial.condition.value}')
        
        # Define position types and their labels
        position_types = [
            ('head_positions', 'Head'),
            ('dominant_foot_positions', 'Dominant Foot'),
            ('non_dominant_foot_positions', 'Non-Dominant Foot'),
            ('hip_positions', 'Hip'),
            ('opponent_hip_positions', 'Opponent Hip')
        ]
        
        # Plot each position type
        for ax, (pos_attr, label) in zip(axes, position_types):
            positions = getattr(trial, pos_attr)
            if not positions:
                ax.text(0.5, 0.5, 'No data available', 
                       horizontalalignment='center',
                       verticalalignment='center',
                       transform=ax.transAxes)
                ax.set_title(f'{label} Position (No Data)')
                continue
                
            null_flags = TrialVisualizer.detect_null_positions(positions)
            
            # Create time array
            times = np.array(trial.timestamps)
            
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
                    null_regions.append((start_idx, i))
                    start_idx = None
            
            # Add the last region if it exists
            if start_idx is not None:
                null_regions.append((start_idx, len(null_flags)))
            
            # Shade null regions
            for start, end in null_regions:
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
    def analyze_and_plot_all_trials(trials: List[Trial], output_dir: str = "output/quality_plots") -> List[str]:
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
            filepath = TrialVisualizer.plot_trial_data_quality(trial, output_dir)
            saved_files.append(filepath)
        return saved_files

    @staticmethod
    def print_trial_quality_summary(trials: List[Trial]) -> None:
        """
        Print a summary of data quality for all trials.
        
        Args:
            trials: List of trials to analyze
        """
        print("\nTrial Data Quality Summary:")
        print("-" * 80)
        print(f"{'Trial':<8} {'Participant':<10} {'Condition':<15} {'Head':<8} {'Dom Foot':<10} "
              f"{'Non-Dom Foot':<12} {'Hip':<8} {'Opp Hip':<10}")
        print("-" * 80)
        
        for trial in trials:
            # Calculate null percentages for each position type
            head_null = TrialVisualizer.calculate_null_percentage(trial.head_positions)
            dom_foot_null = TrialVisualizer.calculate_null_percentage(trial.dominant_foot_positions)
            non_dom_foot_null = TrialVisualizer.calculate_null_percentage(trial.non_dominant_foot_positions)
            hip_null = TrialVisualizer.calculate_null_percentage(trial.hip_positions)
            opp_hip_null = TrialVisualizer.calculate_null_percentage(trial.opponent_hip_positions)
            
            print(f"{trial.trial_number:<8} {trial.participant_id:<10} {trial.condition.value:<15} "
                  f"{head_null:>6.1f}% {dom_foot_null:>8.1f}% {non_dom_foot_null:>10.1f}% "
                  f"{hip_null:>6.1f}% {opp_hip_null:>8.1f}%")
        
        print("-" * 80) 