from pathlib import Path
import numpy as np

from .domain import TrialCollection, Persistence
from .persistence import Table


def table_descriptive_statistics(trials: TrialCollection, file_name: Path, persistence: Persistence) -> None:
    variables = {
        "Number of touches [N]": lambda t: t.number_of_touches(),
        "Duration [s]": lambda t: t.duration(),
        "Distance between last touch and pass [m]": lambda t: t.distance_between_last_touch_and_pass(),
        "Time between last change of direction and pass [s]": lambda t: t.time_between_last_change_of_direction_and_pass(),
        "Number of lateral changes of direction [N]": lambda t: t.number_lateral_changes_of_direction()
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
            f"{mean:.2f}",
            f"{std:.2f}",
            f"{min_val:.2f}",
            f"{max_val:.2f}"
        ])
    
    table = Table(
        title=f"Descriptive Statistics (N = {n_participants})",
        header=header,
        rows=rows
    )
    
    persistence.save_table(table, file_name) 