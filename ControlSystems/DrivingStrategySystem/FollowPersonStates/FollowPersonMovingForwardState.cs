using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonMovingForwardState : DrivingState<FollowPersonMovingForwardState>
{


    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Drive(system, Direction.Forward, 0.5); // Move forward

        double currentDistance = drivingContext.ObstacleDetectionSystem.Distance;
        if (!drivingContext.IsDistanceChanging(currentDistance, drivingContext.PreviousDistance))
        {
            system.SetState(FollowPersonStoppingState.Instance); // Pass the context to next state
        }

        drivingContext.PreviousDistance = currentDistance; // Update previous distance
    }
}

