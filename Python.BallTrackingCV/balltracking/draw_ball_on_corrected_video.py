import cv2
import numpy as np

def draw_ball_on_video(video_path, transformed_positions, homography_matrix, output_video_path, dst_pts,
                       ball_color=(0, 0, 255), ball_radius=10):
    # Open the video capture
    cap = cv2.VideoCapture(video_path)

    # Calculate the dimensions from dst_pts
    width = int(max(dst_pts[:, 0]))
    height = int(max(dst_pts[:, 1]))

    # Get the FPS from the original video
    fps = int(cap.get(cv2.CAP_PROP_FPS))

    # Initialize VideoWriter with the new width and height from dst_pts
    out = cv2.VideoWriter(output_video_path, cv2.VideoWriter_fourcc(*'mp4v'), fps, (width, height))

    frame_idx = 0

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        # Apply homography to the entire frame, resizing it to the new width and height
        warped_frame = cv2.warpPerspective(frame, homography_matrix, (width, height))

        # Draw ball on the frame if there is a valid position for that frame
        frame_key = f"frame_{frame_idx:04d}"
        if frame_key in transformed_positions and transformed_positions[frame_key]:
            ball_position = transformed_positions[frame_key]
            x, y = int(ball_position['x']), int(ball_position['y'])

            # Ensure the ball is within the bounds of the frame after transformation
            if 0 <= x < width and 0 <= y < height:
                # Draw the larger red dot on the warped frame
                cv2.circle(warped_frame, (x, y), ball_radius, ball_color, -1)

        # Write the frame to the output video
        out.write(warped_frame)
        frame_idx += 1

    cap.release()
    out.release()

    print(f"Video with annotations saved at {output_video_path}")
