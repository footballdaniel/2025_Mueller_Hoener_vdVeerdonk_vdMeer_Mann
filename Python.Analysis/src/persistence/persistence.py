from abc import ABC, abstractmethod
from pathlib import Path
from typing import List

from matplotlib.figure import Figure

from src.domain import Trial
from src.persistence.column_format import ColumnFormat
from src.persistence.table import Table


class Persistence(ABC):

    @abstractmethod
    def figure_width(self, column_format: ColumnFormat) -> float:
        ...

    @abstractmethod
    def save_table(self, table: Table, filename: Path, column_format: ColumnFormat = ColumnFormat.DOUBLE) -> None:
        ...

    @abstractmethod
    def save_figure(self, figure: Figure, filename: Path) -> None:
        ...

    @abstractmethod
    def figure_width(self, column_format: ColumnFormat) -> float:
        ...

    @abstractmethod
    def save_text(self, model: str, file_name: Path) -> None:
        ...

    @abstractmethod
    def save_outliers(self, outliers: List[Trial], file_name: Path) -> None:
        ...
