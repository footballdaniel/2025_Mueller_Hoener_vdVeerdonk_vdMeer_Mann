from typing import List, Dict, Tuple
import matplotlib.pyplot as plt
import numpy as np
import os
from pathlib import Path
from src.domain.trial import Trial
from src.domain.position import Position
from src.domain.enums import Condition


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
        if not positions:
            return []
            
        null_flags = [False] * len(positions)
        for i in range(len(positions) - 1, 0, -1):
            if positions[i].distance_2d(positions[i - 1]) == 0:
                null_flags[i] = True
        return null_flags

    @staticmethod
    def calculate_null_percentage(positions: List[Position]) -> float:
        if not positions:
            return 100.0
            
        null_flags = TrialVisualizer.detect_null_positions(positions)
        return (sum(null_flags) / len(null_flags)) * 100

    @staticmethod
    def plot_trial_data_quality(trial: Trial, output_dir: str, ignore_opponent_in_no_opponent: bool = True) -> None:
        os.makedirs(output_dir, exist_ok=True)
        
        timestamps, head_pos, dom_foot_pos, non_dom_foot_pos, hip_pos, opp_hip_pos = TrialVisualizer.get_trial_segment(trial)
        
        has_missing = any([
            TrialVisualizer.has_missing_data(head_pos),
            TrialVisualizer.has_missing_data(dom_foot_pos),
            TrialVisualizer.has_missing_data(non_dom_foot_pos),
            TrialVisualizer.has_missing_data(hip_pos),
            (not ignore_opponent_in_no_opponent or trial.condition != Condition.NO_OPPONENT) and 
            TrialVisualizer.has_missing_data(opp_hip_pos)
        ])
        
        if not has_missing:
            return
        
        fig, axes = plt.subplots(5, 1, figsize=(15, 12), sharex=True)
        fig.suptitle(f'Trial {trial.trial_number} - Participant {trial.participant_id} - {trial.condition.value}')
        
        position_types = [
            (head_pos, 'Head'),
            (dom_foot_pos, 'Dominant Foot'),
            (non_dom_foot_pos, 'Non-Dominant Foot'),
            (hip_pos, 'Hip'),
            (opp_hip_pos, 'Opponent Hip')
        ]
        
        for ax, (positions, label) in zip(axes, position_types):
            if not positions:
                ax.text(0.5, 0.5, 'No data available', 
                       horizontalalignment='center',
                       verticalalignment='center',
                       transform=ax.transAxes)
                ax.set_title(f'{label} Position (No Data)')
                continue
                
            null_flags = TrialVisualizer.detect_null_positions(positions)
            times = np.array(timestamps)
            
            ax.plot(times, [pos.x for pos in positions], 'b-', label='X', alpha=0.7)
            ax.plot(times, [pos.y for pos in positions], 'g-', label='Y', alpha=0.7)
            ax.plot(times, [pos.z for pos in positions], 'r-', label='Z', alpha=0.7)
            
            null_regions = []
            start_idx = None
            for i, is_null in enumerate(null_flags):
                if is_null and start_idx is None:
                    start_idx = i
                elif not is_null and start_idx is not None:
                    null_regions.append((start_idx, i - 1))
                    start_idx = None
            
            if start_idx is not None:
                null_regions.append((start_idx, len(null_flags) - 1))
            
            for start, end in null_regions:
                if start <= end < len(times):
                    ax.axvspan(times[start], times[end], color='red', alpha=0.2)
            
            null_percentage = TrialVisualizer.calculate_null_percentage(positions)
            ax.set_title(f'{label} Position (Null: {null_percentage:.1f}%)')
            ax.set_ylabel('Position (m)')
            ax.grid(True, alpha=0.3)
            ax.legend()
        
        axes[-1].set_xlabel('Time (s)')
        plt.tight_layout()
        
        filename = f"trial_{trial.trial_number}_participant_{trial.participant_id}_{trial.condition.value.lower()}.png"
        filepath = os.path.join(output_dir, filename)
        
        plt.savefig(filepath, dpi=300, bbox_inches='tight')
        plt.close()

    @staticmethod
    def analyze_and_plot_all_trials(trials: List[Trial], output_dir: str = "output/quality_plots", ignore_opponent_in_no_opponent: bool = True) -> None:
        for trial in trials:
            TrialVisualizer.plot_trial_data_quality(trial, output_dir, ignore_opponent_in_no_opponent)

    @staticmethod
    def print_trial_quality_summary(trials: List[Trial], ignore_opponent_in_no_opponent: bool = True) -> None:
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