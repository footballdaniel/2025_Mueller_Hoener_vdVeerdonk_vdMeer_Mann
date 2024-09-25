import cv2
import numpy as np
import torch

from balltracking.apply_homography_to_ball_positions import apply_homography_to_ball_positions
from balltracking.draw_ball_on_corrected_video import draw_ball_on_video
from balltracking.load_ball_positions import load_ball_positions
from balltracking.process_video import process_video

# Example usage
video_path = 'football_2.mp4'  # Path to the input video
output_json = 'ball_positions.json'  # Output JSON file
output_frames_dir = 'frames'  # Directory to save frames
output_video_path = 'output_with_annotations.mp4'  # Output annotated video

model = torch.hub.load('ultralytics/yolov5', 'yolov5s', pretrained=True)
model.conf = 0.05  # confidence threshold (0-1)
model.iou = 0.1  # NMS IoU threshold (0-1)
process_video(model, video_path, output_json, output_frames_dir, save_video=True, output_video_path=output_video_path)

# Custom points detected as field corners
bottom_left = [330, 460]
bottom_right = [950, 930]
top_right = [1480, 547]
top_left = [848, 400]

if video_path == 'football_2.mp4':
    bottom_left = [280, 546]
    bottom_right = [930, 1007]
    top_right = [1444, 585]
    top_left = [809, 463]

source_points = np.array([top_left, top_right, bottom_left, bottom_right], dtype=np.float32)
destination_points = np.array([[0, 0], [500, 0], [0, 500], [500, 500]], dtype=np.float32)
homography_matrix, _ = cv2.findHomography(source_points, destination_points)
ball_positions = load_ball_positions('ball_positions.json')
transformed_ball_positions = apply_homography_to_ball_positions(ball_positions, homography_matrix)
draw_ball_on_video(video_path, transformed_ball_positions, homography_matrix, output_video_path, destination_points)

