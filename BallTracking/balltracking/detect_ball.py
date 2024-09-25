from typing import Optional

from balltracking.domain import Ball


def detect_best_ball(model, image_path) -> Optional[Ball]:
    results = model(image_path)
    best_ball = None

    # Filter the results for class 32 (sports ball)
    for pred in results.xyxy[0]:
        if int(pred[5]) == 32:  # class 32 is the ball
            ball = Ball(
                xmin=float(pred[0]),
                ymin=float(pred[1]),
                xmax=float(pred[2]),
                ymax=float(pred[3]),
                confidence=float(pred[4])
            )
            # Update if no ball is stored yet or if this ball has a better confidence
            if best_ball is None or ball.confidence > best_ball.confidence:
                best_ball = ball

    # Return the best ball or None if no ball was detected
    return best_ball
