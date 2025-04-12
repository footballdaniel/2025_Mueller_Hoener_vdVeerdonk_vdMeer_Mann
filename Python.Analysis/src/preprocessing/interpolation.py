from typing import List

from src.domain import Position


class Interpolator:
    @staticmethod
    def try_interpolate_missing_data(positions: List[Position], ignore_start: bool = False, ignore_end: bool = False,
                                     max_missing_percentage: float = 0.2) -> bool:
        lost_tracking_flags = [False] * len(positions)
        for i in range(len(positions) - 1, 0, -1):

            if ignore_start and positions[i].distance_2d(positions[0]) == 0:
                continue

            if ignore_end and positions[i].distance_2d(positions[-1]) == 0:
                continue

            if positions[i].distance_2d(positions[i - 1]) == 0:
                lost_tracking_flags[i] = True

        if any(lost_tracking_flags):
            derivative = [lost_tracking_flags[i] - lost_tracking_flags[i - 1] for i in
                          range(1, len(lost_tracking_flags))]
            derivative.insert(-1, 0)
            start_indices = [i for i, x in enumerate(derivative) if x == 1]
            end_indices = [min(i + 1, len(positions) - 1) for i, x in enumerate(derivative) if x == -1]

            for start, end in zip(start_indices, end_indices):
                start_position = positions[start]
                end_position = positions[end]
                for i in range(start, end):
                    positions[i] = start_position.interpolate(end_position, (i - start) / (end - start))

        # calc percentage missing
        missing_data = sum(lost_tracking_flags)
        if missing_data == 0:
            return True
        percentage_missing_data = missing_data / len(positions)
        return percentage_missing_data < max_missing_percentage
