import json
import os
import cv2
from balltracking.detect_ball import detect_best_ball
from balltracking.extract_frames import extract_frames
from balltracking.save_annotated_video import save_annotated_video

def process_video(model, video_path, output_json, output_frames_dir="frames", save_video=False, output_video_path=None):
    # Step 1: Extract frames
    frames = extract_frames(video_path, output_frames_dir)

    # Step 2: Process each frame and detect the best ball
    all_ball_positions = {}
    annotated_frames = []

    for frame_path in frames:
        frame_idx = os.path.basename(frame_path).split('.')[0]
        best_ball = detect_best_ball(model, frame_path)

        if best_ball:
            # Store the Ball dataclass instance directly in JSON format
            all_ball_positions[frame_idx] = best_ball.__dict__
        else:
            all_ball_positions[frame_idx] = None  # No ball found in this frame

        if save_video:
            # Load the original frame
            frame = cv2.imread(frame_path)

            # Draw bounding box around the detected ball if found
            if best_ball:
                cv2.rectangle(
                    frame,
                    (int(best_ball.xmin), int(best_ball.ymin)),
                    (int(best_ball.xmax), int(best_ball.ymax)),
                    (255, 0, 0), 2  # Blue box for the ball
                )

            annotated_frames.append(frame)

    # Step 3: Save results to a JSON file
    with open(output_json, 'w') as f:
        json.dump(all_ball_positions, f, indent=4)

    print(f"Results saved to {output_json}")

    # Step 4 (Optional): Save the annotated video
    if save_video and annotated_frames:
        save_annotated_video(annotated_frames, output_video_path, video_path)
