from .post_hoc import duration_and_touches_post_hoc
from .predictive_figure_duration_and_touches import duration_and_touches_predictive_figure
from .regression_duration import regression_duration
from .regression_touches import regression_touches
from .table_duration_and_touches import duration_and_touches_table

__all__ = [
    'regression_duration',
    'regression_touches',
    'duration_and_touches_table',
    'duration_and_touches_predictive_figure',
    'duration_and_touches_post_hoc',
]
