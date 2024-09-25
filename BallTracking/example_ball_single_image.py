import torch

# Model
model = torch.hub.load('ultralytics/yolov5', 'yolov5s', pretrained=True)

# Images
imgs = ['football_1.png']  # batch of images

# Inference
results = model(imgs)

# Results
results.print()
results.show()
# results.save(save_dir=".")  # save as 'results.png'

print(results.xyxy[0])  # img1 predictions (tensor)
print(results.pandas().xyxy[0])  # img1 predictions (pandas)

#           xmin        ymin         xmax        ymax  confidence  class  \
# 0  1001.718445  415.601898  1099.527100  665.112854    0.853806      0
# 1   112.334656  259.858063   164.286636  388.465790    0.571692      0
# 2   993.454834  643.224243  1028.561279  670.840271    0.542021     32
# 3    32.279766  262.265717    80.198898  340.202087    0.291808      0
#
#           name
# 0       person
# 1       person
# 2  sports ball
# 3       person