from typing import Dict, List, Tuple
import matplotlib.pyplot as plt
import numpy as np
from src.domain import TrialCollection, Condition


class TrialAnalyzer:
    @staticmethod
    def analyze_participant_trials(trials: TrialCollection) -> Dict[int, Dict[Condition, int]]:
        """
        Analyze how many trials each participant has in each condition.
        
        Args:
            trials: Collection of trials to analyze
            
        Returns:
            Dictionary mapping participant IDs to their trial counts per condition
        """
        participant_counts: Dict[int, Dict[Condition, int]] = {}
        
        for trial in trials:
            participant_id = trial.participant_id
            if participant_id not in participant_counts:
                participant_counts[participant_id] = {condition: 0 for condition in Condition}
            participant_counts[participant_id][trial.condition] += 1
            
        return participant_counts

    @staticmethod
    def print_participant_analysis(trials: TrialCollection) -> None:
        """
        Print analysis of participant trial counts.
        
        Args:
            trials: Collection of trials to analyze
        """
        participant_counts = TrialAnalyzer.analyze_participant_trials(trials)
        
        print("\nParticipant Trial Analysis:")
        print("-" * 50)
        print(f"{'Participant ID':<15} {'In Situ':<10} {'Interaction':<12} {'No Interaction':<15} {'No Opponent':<12}")
        print("-" * 50)
        
        for participant_id, counts in sorted(participant_counts.items()):
            print(f"{participant_id:<15} {counts[Condition.IN_SITU]:<10} {counts[Condition.INTERACTION]:<12} "
                  f"{counts[Condition.NO_INTERACTION]:<15} {counts[Condition.NO_OPPONENT]:<12}")
        
        print("-" * 50)
        print(f"Total Participants: {len(participant_counts)}")
        print(f"Expected Trials per Condition: 10")
        print("-" * 50)

    @staticmethod
    def create_participation_grid(trials: TrialCollection) -> None:
        """
        Create a grid visualization of participant participation across conditions.
        Similar to GitHub's contribution graph.
        
        Args:
            trials: Collection of trials to visualize
        """
        participant_counts = TrialAnalyzer.analyze_participant_trials(trials)
        
        # Create a matrix for the grid
        n_participants = len(participant_counts)
        n_conditions = len(Condition)
        grid = np.zeros((n_participants, n_conditions))
        
        # Fill the grid with trial counts
        for i, (participant_id, counts) in enumerate(sorted(participant_counts.items())):
            for j, condition in enumerate(Condition):
                grid[i, j] = counts[condition]
        
        # Create the plot
        fig, ax = plt.subplots(figsize=(10, n_participants * 0.5))
        
        # Create the heatmap
        im = ax.imshow(grid, aspect='auto', cmap='YlOrRd')
        
        # Customize the plot
        ax.set_xticks(np.arange(n_conditions))
        ax.set_yticks(np.arange(n_participants))
        ax.set_xticklabels([condition.value for condition in Condition])
        ax.set_yticklabels([f"P{pid}" for pid in sorted(participant_counts.keys())])
        
        # Rotate the x-axis labels
        plt.setp(ax.get_xticklabels(), rotation=45, ha="right", rotation_mode="anchor")
        
        # Add colorbar
        plt.colorbar(im, ax=ax, label='Number of Trials')
        
        # Add title
        plt.title('Participant Trial Distribution Across Conditions')
        
        # Adjust layout
        plt.tight_layout()
        
        # Show the plot
        plt.show()

    @staticmethod
    def analyze_completeness(trials: TrialCollection) -> None:
        """
        Analyze and print information about trial completeness.
        
        Args:
            trials: Collection of trials to analyze
        """
        participant_counts = TrialAnalyzer.analyze_participant_trials(trials)
        
        # Check for participants with complete data
        complete_participants = []
        incomplete_participants = []
        
        for participant_id, counts in participant_counts.items():
            if all(counts[condition] == 10 for condition in Condition):
                complete_participants.append(participant_id)
            else:
                incomplete_participants.append(participant_id)
        
        print("\nTrial Completeness Analysis:")
        print("-" * 50)
        print(f"Total Participants: {len(participant_counts)}")
        print(f"Participants with Complete Data: {len(complete_participants)}")
        print(f"Participants with Incomplete Data: {len(incomplete_participants)}")
        
        if incomplete_participants:
            print("\nIncomplete Participants:")
            for pid in incomplete_participants:
                counts = participant_counts[pid]
                missing = [cond.value for cond, count in counts.items() if count < 10]
                print(f"Participant {pid}: Missing {missing}")
        
        print("-" * 50) 