from pathlib import Path
import numpy as np

from src.domain import TrialCollection, Condition
from src.persistence.persistence import Persistence
from src.persistence.apa_word_table_formatter import Table

from src.services import DistanceCalculator, TimeCalculator, MovementCalculator, OutlierCalculator


def table_descriptive_statistics(trials: TrialCollection, file_name: Path, persistence: Persistence) -> None:
    variables = {
        "Number of touches [N]": MovementCalculator.number_of_touches,
        "Trial duration [s]": TimeCalculator.duration,
        "Distance between last touch and pass [m]": DistanceCalculator.distance_between_last_touch_and_pass,
        "Time between last change of direction and pass [s]": TimeCalculator.time_between_last_change_of_direction_and_pass,
        "Number of changes of direction [N]": MovementCalculator.number_lateral_changes_of_direction
    }

    participant_ids = {trial.participant_id for trial in trials}
    n_participants = len(participant_ids)
    
    header = ["Variable", "Condition", "Mean", "SD", "Min", "Max"]
    rows = []
    
    condition_order = [
        Condition.NoOpponent,
        Condition.NoInteraction,
        Condition.Interaction,
        Condition.InSitu
    ]
    
    for var_name, var_func in variables.items():
        all_values = [var_func(trial) for trial in trials]
        overall_mean = np.mean(all_values)
        overall_std = np.std(all_values)
        overall_min = np.min(all_values)
        overall_max = np.max(all_values)
        
        rows.append([
            var_name,
            "",
            "",
            "",
            "",
            ""
        ])
        
        for condition in condition_order:
            condition_trials = [trial for trial in trials if trial.condition == condition]
            if len(condition_trials) > 0:
                condition_values = [var_func(trial) for trial in condition_trials]
                condition_mean = np.mean(condition_values)
                condition_std = np.std(condition_values)
                condition_min = np.min(condition_values)
                condition_max = np.max(condition_values)
                
                rows.append([
                    "",
                    str(condition),
                    f"{condition_mean:.1f}",
                    f"{condition_std:.1f}",
                    f"{condition_min:.1f}",
                    f"{condition_max:.1f}"
                ])
        
        rows.append([
            "",
            "Across conditions",
            f"{overall_mean:.1f}",
            f"{overall_std:.1f}",
            f"{overall_min:.1f}",
            f"{overall_max:.1f}"
        ])
    
    table = Table(
        title=f"Descriptive Statistics (N = {n_participants})",
        header=header,
        rows=rows
    )

    duration_threshold = 10
    touches_threshold = 10
    outliers_duration = OutlierCalculator.duration_greater_than(trials, duration_threshold)
    outliers_touches = OutlierCalculator.number_of_touches_greater_than(trials, touches_threshold)

    persistence.save_text(f"Outliers: {outliers_duration} trials with duration > {duration_threshold}s, "
                          f"{outliers_touches} trials with touches > {touches_threshold}.\n\n", file_name)


    
    persistence.save_table(table, file_name) 