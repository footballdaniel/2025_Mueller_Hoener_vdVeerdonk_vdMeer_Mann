from pathlib import Path

import pandas as pd

from src.clustering import plot_cluster_distribution, perform_cluster_analysis, plot_elbow_method
from src.persistence import ApaStyledPersistence
from src.preprocessing import TrialReader
from src.regressions import (
    regression_duration,
    regression_touches,
    duration_and_touches_predictive_figure,
    duration_and_touches_table,
    duration_and_touches_post_hoc
)
from src.descriptive_statistics import table_descriptive_statistics
from src.combined_figures import combined_predictive_and_cluster_figure

if __name__ == '__main__':
    data_path = "../Data/Experiment/**/*.csv"

    persistence = ApaStyledPersistence(
        font=Path("Calibri.ttf"),
        font_size=11,
        double_column_width_inches=6.5,
        single_column_width_inches=3,
        grayscale=False
    )

    pd.set_option('display.max_columns', None)  # Show all columns
    pd.set_option('display.width', 1000)  # Set a wider width

    trials = TrialReader.read_trials(data_path)


    table_descriptive_statistics(trials, Path("descriptive_statistics.docx"), persistence)

    regression_duration(trials, Path("model_1.nc"), Path("model_1.txt"), persistence)
    regression_touches(trials, Path("model_2.nc"), Path("model_2.txt"), persistence)

    plot_elbow_method(trials, max_clusters=10, persistence=persistence, path=Path("elbow_method_results.txt"))
    perform_cluster_analysis(trials, n_clusters=3)
    plot_cluster_distribution(trials, persistence=persistence)

    duration_and_touches_table(Path("model_1.nc"), Path("model_2.nc"), Path("model_predictions.docx"), persistence)
    duration_and_touches_predictive_figure(Path("model_1.nc"), Path("model_2.nc"), Path("predictions.png"), persistence)
    duration_and_touches_post_hoc(Path("model_1.nc"), Path("model_2.nc"), Path("ridge_differences.png"), persistence)

    combined_predictive_and_cluster_figure(
        Path("model_1.nc"),
        Path("model_2.nc"),
        trials,
        Path("combined_figure.png"),
        persistence
    )
