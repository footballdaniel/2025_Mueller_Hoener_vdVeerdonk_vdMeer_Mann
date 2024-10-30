import csv
import json

from src.domain import Position, Side, Foot, Pass, Touch
from src.dtos import FootPositionsOverTime, FootPositionAtFrame


def read_foot_positions(json_positions_data: dict) -> FootPositionsOverTime:
    """
    Reads foot positions from JSON data and returns a FootPositionsOverTime object.
    """
    positions = []
    for frame_number_str, position_data in json_positions_data.items():
        frame_number = int(frame_number_str)
        position = Position(
            x=position_data.get("x", 0.0),
            y=position_data.get("y", 0.0),
            z=position_data.get("z", 0.0)
        )
        positions.append(FootPositionAtFrame(
            frame_number=frame_number,
            position=position
        ))
    return FootPositionsOverTime(positions=positions)


def process_file(csv_file: str, json_file: str):
    """
    Process a single pair of CSV and JSON files.
    """
    with open(json_file, "r") as json_file_content:
        json_data = json.load(json_file_content)

    dominant_foot_str = json_data.get("DominantFoot", "Right")
    is_dominant_foot_right = dominant_foot_str == "Right"

    # Read foot positions into data classes
    dominant_foot_positions = read_foot_positions(json_data.get("UserDominantFootPositions", {}))
    non_dominant_foot_positions = read_foot_positions(json_data.get("UserNonDominantFootPositions", {}))

    with open(csv_file, "r") as csv_file_content:
        csv_reader = csv.reader(csv_file_content)
        for row in csv_reader:
            if not row:
                continue  # Skip empty lines

            frame_number_str = row[0].strip()
            event_tag = row[1].strip()

            frame_number = int(frame_number_str)

            # Determine the side based on the event tag
            side = Side.RIGHT if "R" in event_tag else Side.LEFT

            # Get the foot position
            if (side == Side.RIGHT and is_dominant_foot_right) or (side == Side.LEFT and not is_dominant_foot_right):
                # Use dominant foot positions
                foot_position = dominant_foot_positions.get_position_at_frame(frame_number)
            else:
                # Use non-dominant foot positions
                foot_position = non_dominant_foot_positions.get_position_at_frame(frame_number)

            # Determine if the foot is dominant
            is_dominant = (side == Side.RIGHT and is_dominant_foot_right) or \
                          (side == Side.LEFT and not is_dominant_foot_right)

            foot = Foot(
                side=side,
                position=foot_position,
                is_dominant=is_dominant
            )

            # Process event tags to determine action
            if "P" in event_tag:
                # It's a pass event
                success = "N" not in event_tag  # 'N' indicates an unsuccessful pass
                pass_action = Pass(
                    position=foot_position,
                    time=float(frame_number),
                    foot=foot,
                    success=success
                )
                # Handle the pass action (e.g., store or print)
                print(f"Pass action at frame {frame_number}: {pass_action}")
            else:
                # It's a touch event
                touch_action = Touch(
                    position=foot_position,
                    time=float(frame_number),
                    foot=foot
                )
                # Handle the touch action (e.g., store or print)
                print(f"Touch action at frame {frame_number}: {touch_action}")
