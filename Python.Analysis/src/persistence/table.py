from dataclasses import dataclass
from typing import List, Union


@dataclass
class Table:
    title: str
    header: List[str]
    rows: List[List[Union[str, float]]]

    def rename_element(self, original: str, new: str):
        if original in self.header:
            self.header[self.header.index(original)] = new
        for row in self.rows:
            if original in row:
                row[row.index(original)] = new

    def reorder_element(self, cell_content: str, new_row_id: int):
        # Separate rows containing `name` and those that do not
        matching_rows = [row for row in self.rows if cell_content in row]
        non_matching_rows = [row for row in self.rows if cell_content not in row]
        self.rows = non_matching_rows[:new_row_id] + matching_rows + non_matching_rows[new_row_id:]

    def __post_init__(self):
        for row in self.rows:
            if len(row) != len(self.header):
                raise Exception(
                    f"Number of columns in each row must match the number of header elements: error in row: {row}")
