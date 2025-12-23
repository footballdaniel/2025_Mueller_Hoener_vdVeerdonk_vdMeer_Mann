# Interactive decision making

## Aim:
Measure proximity to real life movements. If i can show that movements in "reactive" vr are less pronounced, then i am already off to a good start.

## Contexts:
- In sports, commercially available "interactive" environments have been used
    - Michalski
    - Tirp
- "Social forces", "Multi agent behavior prediction", and "Trajectory prediction", "Obstacle avoidance"
    - modelling for pedestrian, cars, bikes
    - Rudenko, Ridel
- Lead in and Follow through movements


## Articles
- Zou et al (2020): Pattern based approaches
    - Using clustering, associative rules, sequence matching (e.g. citation 26, morris and trivedi)
        - Downside: only looks at historical data, does not take into account social factors
    - Zou managed to quantify the attention that each pedestrian gives to the environment!
- Ivanovic (2020): Average displacement error / Final displacement error
    - Better: "Best of N" I have a ground truth for a trajectory. I can compare many trajectories of a player and pick the "Best of N" (BoN).
    - Even better: Kernel density for time horizon


## Behavioral measures
- Ridel et al (2018): When interacting with a car that goes in parallel with their path, people turn their heads, which conveys their intention.
    - They move slower than if the car was not there (physics models failed)
    - Models of gait initiation
    - Quantifying interactions: When crossing one way streets, people interact less with cars (check citation 58)
    - Pedestrian orientation (check citation 8)
    - Switching between 3 different models: (1) Critical minimal distance, (2) orientation/awareness (3) distance from curbside
    - Modelling when a pedestrian stops, is hard (Hashimoto, 54)
    - Shift of speed
    - Reasons for abrupt stops (citation 70)
- Zou et al (2020): Attention radius of a pedestrian (p. 10)

- Rudenko (2019): 
    - Concepts
        - Very zoomed out: (1) modeling approach and (2) contextual cues are very different parts of the problem
        - Horizons: Short term predictions (1-2 s), long term (<20 s>)
        - I probably want to do "Pattern-based modeling" (learn prototypical features). 
        - In the future, I want to do "Planning based" (learn how to figure out the goal of an opponent)
    - Contextual cues 
        - Position, velocity, articulated pose of the target agent

    - Other reviews mentioned: 
        - Lasota (2017): "Goal intent techniques" trying to infer the goal of an agent
        - Lefevre (2014): (1) Maneuvers (2) Physics (3) Interaction aware
        - Brouwer (2016): (1) Dynamics based models (2) psychological, probabilistic knowledge, (3) head orientation
        - Schneider & Gavrila (2013): Constant velocity, constant acceleration, constant turn

- Rauter et al. (2013): Transfer of Complex Skill Learning from Virtual to Real Rowing
    - Coordination of body segments
    - Temporal overlap of legs and trunk
    - Catch angle, release angle, stroke length, power
- Tirp et al. (2015): Transfer of dart throwing skill from VR to real throwing 
    - Xbox Connect


## Presentation
# Oli: 
- Interaction zone
- Black box commercial applications

- Pedestrians
- Orientation => Intent (more looking at opponets that you interact with)
- Expected body orientation when passing (robotics)
- slowing down => Interaction (delay)
- Korridor/passing studien

- Model:
- Showing that correspondence is ~1, and decreasing as a function of IPD
    - What is the average reaction time?
    - Show that shifted autocorrelation models enable us to identify active components of interactions