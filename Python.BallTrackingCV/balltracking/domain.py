from dataclasses import dataclass


@dataclass
class Ball:
    xmin: float
    ymin: float
    xmax: float
    ymax: float
    confidence: float
    class_id: int = 32

    @property
    def x_center(self):
        return (self.xmin + self.xmax) / 2

    @property
    def y_center(self):
        return self.ymin
