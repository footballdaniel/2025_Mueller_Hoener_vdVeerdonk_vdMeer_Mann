import unittest

from src.domain import Position
from src.interpolation import Interpolator


class TestInterpolator(unittest.TestCase):
    def test_interpolation_no_gaps(self):
        positions = [Position(x=i, y=i, z=i) for i in range(10)]
        positions[1] = positions[0]  # pretend tracker was not updated this frame
        Interpolator.try_interpolate_missing_data(positions)
        self.assertEqual(positions, [Position(x=i, y=i, z=i) for i in range(10)])

    def test_interpolation_with_longer_gaps(self):
        positions = [Position(x=i, y=i, z=i) for i in range(10)]
        positions[1] = positions[0]  # pretend tracker was not updated this frame
        positions[2] = positions[0]  # pretend tracker was not updated this frame
        Interpolator.try_interpolate_missing_data(positions)
        self.assertEqual(positions, [Position(x=i, y=i, z=i) for i in range(10)])

    def test_interpolation_with_longer_gaps_at_end(self):
        positions = [Position(x=i, y=i, z=i) for i in range(10)]
        positions[8] = positions[7]  # pretend tracker was not updated this frame
        Interpolator.try_interpolate_missing_data(positions)
        self.assertEqual(positions, [Position(x=i, y=i, z=i) for i in range(10)])

    def test_interpolation_edge_case(self):
        positions = [Position(x=0, y=0, z=0)] * 10
        Interpolator.try_interpolate_missing_data(positions)
        self.assertTrue(all(p == Position(x=0, y=0, z=0) for p in positions))
