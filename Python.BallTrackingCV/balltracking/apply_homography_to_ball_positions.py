import cv2
import numpy as np


def apply_homography_to_ball_positions(ball_positions, homography_matrix):
    transformed_positions = {}

    for frame, ball in ball_positions.items():
        if ball:
            # Extract the center of the ball (average of xmin, xmax and ymin)
            x_center = (ball['xmin'] + ball['xmax']) / 2
            y_center = ball['ymin']
            original_point = np.array([[[x_center, y_center]]], dtype=np.float32)

            # Apply the homography transformation
            transformed_point = cv2.perspectiveTransform(original_point, homography_matrix)
            transformed_positions[frame] = {
                'x': transformed_point[0][0][0],
                'y': transformed_point[0][0][1],
                'confidence': ball['confidence']
            }
        else:
            transformed_positions[frame] = None

    return transformed_positions
