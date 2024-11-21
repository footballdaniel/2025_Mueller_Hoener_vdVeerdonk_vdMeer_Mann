from dataclasses import dataclass


@dataclass
class Pass:
    is_a_pass: bool
    pass_id: int
    pass_timestamp: float


@dataclass
class NoPass(Pass):
    is_a_pass: bool = False
    pass_id: int = 0
    pass_timestamp: float = 0.0
