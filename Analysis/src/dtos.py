from _bisect import bisect_left
from dataclasses import dataclass, field
from typing import List

from src.domain import Position


@dataclass(order=True)
class FootPositionAtFrame:
    frame_number: int
    position: Position


@dataclass
class FootPositionsOverTime:
    positions: List[FootPositionAtFrame] = field(default_factory=list)

    def __post_init__(self):
        # Sort the positions by frame_number for efficient lookup
        self.positions.sort(key=lambda fp: fp.frame_number)

    def get_position_at_frame(self, frame_number: int) -> Position:
        # Use binary search for efficient frame lookup
        index = bisect_left([fp.frame_number for fp in self.positions], frame_number)
        if index != len(self.positions) and self.positions[index].frame_number == frame_number:
            return self.positions[index].position
        else:
            # Return a default position or handle as needed
            return Position(x=0.0, y=0.0, z=0.0)


