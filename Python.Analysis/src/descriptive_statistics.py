from pathlib import Path
import numpy as np

from src.domain import TrialCollection
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

    # Get unique participant count
    participant_ids = {trial.participant_id for trial in trials}
    n_participants = len(participant_ids)
    
    # Prepare table data
    header = ["Variable", "Mean", "SD", "Min", "Max"]
    rows = []
    
    for var_name, var_func in variables.items():
        values = [var_func(trial) for trial in trials]
        mean = np.mean(values)
        std = np.std(values)
        min_val = np.min(values)
        max_val = np.max(values)
        
        rows.append([
            var_name,
            f"{mean:.1f}",
            f"{std:.1f}",
            f"{min_val:.1f}",
            f"{max_val:.1f}"
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