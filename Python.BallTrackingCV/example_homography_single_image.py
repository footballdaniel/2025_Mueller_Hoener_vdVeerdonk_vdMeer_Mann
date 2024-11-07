import cv2
import numpy as np
from matplotlib import pyplot as plt

# Load the uploaded image
image_path = 'football_1.png'
img = cv2.imread(image_path)

bottom_left = [330, 460]
bottom_right = [950, 930]
top_right = [1480, 547]
top_left = [848, 400]

# Define 4 source points from the provided image (manually selected from the red dots)
# src_pts = np.array([bottom_left,  bottom_right, top_left, top_right], dtype=np.float32)
src_pts = np.array([top_left, top_right, bottom_left, bottom_right], dtype=np.float32)

dst_pts = np.array([[0, 0], [500, 0], [0, 500], [500, 500]], dtype=np.float32)

# Compute the homography matrix
homography_matrix, _ = cv2.findHomography(src_pts, dst_pts)

# Apply the homography transformation
warped_img = cv2.warpPerspective(img, homography_matrix, (500, 500))

# Show the resulting warped image
plt.imshow(cv2.cvtColor(warped_img, cv2.COLOR_BGR2RGB))
plt.axis('off')

# The ball (from torch model)
ball_point = np.array([[(993 + 1028) / 2, 643]], dtype=np.float32)
transformed_point = cv2.perspectiveTransform(np.array([ball_point]), homography_matrix)

# Draw the ball on the warped image
plt.scatter(transformed_point[0][0][0], transformed_point[0][0][1], c='y', s=150)

plt.show()
