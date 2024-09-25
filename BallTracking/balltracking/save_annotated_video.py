import cv2


def save_annotated_video(frames, output_video_path, original_video_path):
    # Get frame width, height, and FPS from the original video
    cap = cv2.VideoCapture(original_video_path)
    fps = int(cap.get(cv2.CAP_PROP_FPS))
    frame_width = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
    frame_height = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))

    # Define the codec and create VideoWriter object
    out = cv2.VideoWriter(output_video_path, cv2.VideoWriter_fourcc(*'mp4v'), fps, (frame_width, frame_height))

    # Write each annotated frame into the video
    for frame in frames:
        out.write(frame)

    # Release the video writer and capture object
    out.release()
    cap.release()

    print(f"Annotated video saved to {output_video_path}")
