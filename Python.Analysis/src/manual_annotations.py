import csv
import json
import re
from typing import List

from src.domain import Position, Footedness, Foot, Pass, Touch, Trial, NoAction, Side, \
    NoPass, Condition
from src.interpolation import Interpolator


def read_positions(json_positions_data: List[dict]) -> List[Position]:
    positions = []
    for position_data in json_positions_data:
        position = Position(
            x=position_data.get("x", 0.0),
            y=position_data.get("y", 0.0),
            z=position_data.get("z", 0.0)
        )
        positions.append(position)
    return positions


def ingest(csv_file: str, json_file: str) -> Trial:
    """
    Process a single pair of CSV and JSON files.
    """
    with open(json_file, "r") as json_file_content:
        json_data = json.load(json_file_content)

    # get part of the path that has pattern P[0-9] in it. its basically the participant number
    participant_number = int(re.search(r'P([0-9]+)', csv_file).group(1))

    trial_number = json_data.get("TrialNumber", 0)
    path = csv_file

    dominant_foot_str = json_data.get("DominantFoot", "Right")
    is_dominant_foot_right = dominant_foot_str == "Right"
    condition = json_data.get("Condition", "Unknown")
    if condition == "InSitu":
        condition = Condition.IN_SITU
    elif condition == "LaboratoryInteractive":
        condition = Condition.INTERACTION
    elif condition == "LaboratoryNonInteractive":
        condition = Condition.NO_INTERACTION
    elif condition == "LaboratoryNoOpponent":
        condition = Condition.NO_OPPONENT

    timestamps = json_data.get("Timestamps", [])
    user_head_positions = read_positions(json_data.get("UserHeadPositions"))
    user_dominant_foot_positions = read_positions(json_data.get("UserDominantFootPositions"))
    user_non_dominant_foot_positions = read_positions(json_data.get("UserNonDominantFootPositions"))
    user_hip_positions = read_positions(json_data.get("UserHipPositions"))
    opponent_hip_positions = read_positions(json_data.get("OpponentHipPositions"))

    trial = Trial(
        participant_id=participant_number,
        path=path,
        condition=condition,
        trial_number=trial_number,
        timestamps=timestamps,
        head_positions=user_head_positions,
        dominant_foot_positions=user_dominant_foot_positions,
        non_dominant_foot_positions=user_non_dominant_foot_positions,
        hip_positions=user_hip_positions,
        opponent_hip_positions=opponent_hip_positions,
        actions=[],
        start=NoAction(),
        pass_event=NoPass(),
        dominant_foot_side=Footedness.RIGHT if is_dominant_foot_right else Footedness.LEFT
    )

    # if not Interpolator.try_interpolate_missing_data(user_dominant_foot_positions):
    #     trial.has_missing_data = True
    #     print(f"{path}: Dominant foot has lots of missing data in trial: {path}")

    # if not Interpolator.try_interpolate_missing_data(user_non_dominant_foot_positions):
    #     trial.has_missing_data = True
    #     print(f"{path}: Non-dominant foot has lots of missing data in trial: {path}")

    # if not Interpolator.try_interpolate_missing_data(user_hip_positions):
    #     trial.has_missing_data = True
    #     print(f"{path}: User hip has lots of missing data in trial: {path}")

    # if not Interpolator.try_interpolate_missing_data(opponent_hip_positions, ignore_start=True, ignore_end=True):
    #     trial.has_missing_data = True
    #     print(f"{path}: Opponent hip has lots of missing data in trial: {path}")

    with open(csv_file, "r") as csv_file_content:
        csv_reader = csv.reader(csv_file_content)

        pass_action = NoPass()

        for row in csv_reader:
            if not row:
                continue  # Skip empty lines

            frame_number_str = row[0].strip()
            event_tag = row[1].strip()

            frame_number = int(frame_number_str)

            side = Side.UNKNOWN
            if "Right" in event_tag:
                side = Side.DOMINANT if is_dominant_foot_right else Side.NON_DOMINANT
            if "Left" in event_tag:
                side = Side.NON_DOMINANT if is_dominant_foot_right else Side.DOMINANT

            if side == Side.DOMINANT:
                foot_position = user_dominant_foot_positions[frame_number]
            if side == Side.NON_DOMINANT:
                foot_position = user_non_dominant_foot_positions[frame_number]

            if "Right" in event_tag:
                foot = Foot(side=Side.DOMINANT if is_dominant_foot_right else Side.NON_DOMINANT)
            if "Left" in event_tag:
                foot = Foot(side=Side.NON_DOMINANT if is_dominant_foot_right else Side.DOMINANT)

            if "Pass" in event_tag:
                pass_action = Pass(
                    position=foot_position,
                    time_index=int(frame_number_str),
                    foot=foot,
                    success=True
                )
                trial.actions.append(pass_action)

            if "Touch" in event_tag:
                if "Right" in event_tag:
                    touch_action = Touch(
                        position=foot_position,
                        time_index=int(frame_number_str),
                        foot=foot
                    )
                    trial.actions.append(touch_action)

                if "Left" in event_tag:
                    touch_action = Touch(
                        position=foot_position,
                        time_index=int(frame_number),
                        foot=foot
                    )
                    trial.actions.append(touch_action)

            if "Goal" in event_tag:
                pass_action.success = True

            if "Intercepted" in event_tag:
                pass_action.success = False

            if "OffTarget" in event_tag:
                pass_action.success = True  # !!! to keep all conditions comparable

    if condition != Condition.IN_SITU:
        ball_events = json_data.get("BallEvents", [])
        for ball_event in ball_events:
            if ball_event["Name"] == "Intercepted":
                pass_action.success = False

    trial.start = trial.actions[0]
    trial.pass_event = pass_action

    return trial
