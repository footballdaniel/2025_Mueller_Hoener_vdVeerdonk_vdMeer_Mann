from __future__ import annotations

import math
from dataclasses import dataclass


@dataclass
class Position:
    x: float
    y: float
    z: float

    def mirror_x(self) -> Position:
        return Position(x=-self.x, y=self.y, z=self.z)

    def rotate_around_y(self, angle_degrees: float) -> Position:
        """Rotate the position around the y-axis by the given angle in degrees."""
        angle_radians = math.radians(angle_degrees)
        cos_theta = math.cos(angle_radians)
        sin_theta = math.sin(angle_radians)
        x_new = self.x * cos_theta + self.z * sin_theta
        z_new = -self.x * sin_theta + self.z * cos_theta
        return Position(x=x_new, y=self.y, z=z_new)
