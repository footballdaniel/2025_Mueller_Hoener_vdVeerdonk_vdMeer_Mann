import json


def load_ball_positions(json_path):
    with open(json_path, 'r') as f:
        ball_positions = json.load(f)
    return ball_positions
