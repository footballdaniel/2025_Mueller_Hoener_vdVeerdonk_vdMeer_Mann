from pathlib import Path
from typing import List

from matplotlib import pyplot as plt, font_manager as fm
from matplotlib.figure import Figure
from matplotlib.rcsetup import cycler

from src.domain import Trial
from src.persistence.apa_word_table_formatter import ApaWordTableFormatter
from src.persistence.column_format import ColumnFormat
from src.persistence.persistence import Persistence
from src.persistence.table import Table


class ApaStyledPersistence(Persistence):

    def figure_width(self, column_format: ColumnFormat) -> float:
        if column_format == ColumnFormat.DOUBLE:
            return self.double_column_width_inches
        else:
            return self.single_column_width_inches

    def __init__(self, font: Path, font_size: int, double_column_width_inches: float, single_column_width_inches: float, grayscale: bool=False):
        self.double_column_width_inches = double_column_width_inches
        self.single_column_width_inches = single_column_width_inches

        """FONT"""
        if not font.exists():
            plt.rcParams['font.family'] = 'serif'
            print(f"Font file {font} not found. Using serif font.")
        else:
            font_path = font.resolve()
            fm.fontManager.addfont(font_path)
            font_name = fm.FontProperties(fname=str(font_path)).get_name()
            plt.rcParams['font.family'] = font_name
            plt.rcParams['font.size'] = font_size


        """COLORS AND STYLE"""
        red = "#8B0000"
        blue = "#4A90E2"
        plt.rcParams['axes.prop_cycle'] = cycler(color=[blue, red])  # Red and Dark Blue
        plt.rcParams['lines.linewidth'] = 2
        plt.rcParams['figure.dpi'] = 300

        if grayscale:
            plt.rcParams['axes.prop_cycle'] = cycler(color=["0.00", "0.40", "0.60", "0.70"])
            plt.rcParams['image.cmap'] = 'gray'

        """WORD TABLE FORMATTER"""
        self.formatter = ApaWordTableFormatter(
            single_column_width_inches=single_column_width_inches,
            double_column_width_inches=double_column_width_inches,
            font=font.stem,
            font_size=font_size
        )

    def save_outliers(self, outliers: List[Trial], file_name: Path) -> None:
        if not file_name.parent.exists():
            file_name.parent.mkdir(parents=True)

        text = "\n".join([f"{o.participant},{o.trial},{o.advertisement_direction},{o.goalkeeper_position},{o.condition}" for o in outliers])

        with open(file_name, 'w') as file:
            file.write(text)

    def save_table(self, table: Table, file_name: Path, column_format: ColumnFormat = ColumnFormat.DOUBLE):
        if not file_name.parent.exists():
            file_name.parent.mkdir(parents=True)

        self.formatter.create(table, file_name, column_format)

    def save_text(self, model: str, file_name: Path):
        if not file_name.parent.exists():
            file_name.parent.mkdir(parents=True)

        # remove the extension if it exists
        if file_name.suffix:
            file_name = file_name.with_suffix('')

        # add txt
        file_name = file_name.with_suffix('.txt')

        with open(file_name, 'w') as file:
            file.write(model)

    def save_figure(self, figure: Figure, file_name: Path):
        if not file_name.parent.exists():
            file_name.parent.mkdir(parents=True)

        if file_name.suffix:
            file_name = file_name.with_suffix('')

        file_name = file_name.with_suffix('.png')

        figure.savefig(file_name)
