import os

import cv2


def extract_frames(video_path, output_dir):
    # delete directory / create new empty
    if os.path.exists(output_dir):
        for file in os.listdir(output_dir):
            os.remove(os.path.join(output_dir, file))
    else:
        os.makedirs(output_dir)

    cap = cv2.VideoCapture(video_path)
    frame_idx = 0
    frames = []

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        # Save the current frame as an image
        frame_path = os.path.join(output_dir, f"frame_{frame_idx:04d}.png")
        cv2.imwrite(frame_path, frame)
        frames.append(frame_path)
        frame_idx += 1

    cap.release()
    return frames
