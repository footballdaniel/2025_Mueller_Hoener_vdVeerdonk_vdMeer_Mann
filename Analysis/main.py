import abc
import glob
from dataclasses import dataclass
from enum import Enum

data_path = "../Data/Pilot_2/*.csv"


@dataclass
class Position:
    x: float
    y: float
    z: float


class Side(Enum):
    LEFT = 1
    RIGHT = 2


@dataclass
class Foot:
    side: Side
    position: Position


# abstract base class

@dataclass()
class Action(abc.ABC):
    position: Position
    time: float


@dataclass
class Touch(Action):
    foot: Foot


@dataclass
class Pass(Action):
    foot: Foot
    success: bool


files = glob.glob(data_path)

for csv_file in files:
    print(csv_file)

    json_file = csv_file.replace(".csv", ".json")
    with open(json_file, "r") as json_file_content:
        json_text = json_file_content.read()

        with open(csv_file, "r") as csv_file_content:
            for line in json_file_content:
                columns = line.split(",")

                frame_number = int(columns[0].strip())
                foot_side = columns[1].strip()
                side = Side.LEFT if foot_side == "L" else Side.RIGHT

                # get the element "UserDominantFootPositions" from the json file at the same index as frame number
                opponent_dominant_foot_position = Position(
                    json_text["UserDominantFootPositions"][frame_number]["x"],
                    json_text["UserDominantFootPositions"][frame_number]["y"],
                    json_text["UserDominantFootPositions"][frame_number]["z"]
                )

                touch = Touch(
                    position=opponent_dominant_foot_position,
                    time=frame_number,
                    foot=Foot(side=side, position=opponent_dominant_foot_position)
                )
