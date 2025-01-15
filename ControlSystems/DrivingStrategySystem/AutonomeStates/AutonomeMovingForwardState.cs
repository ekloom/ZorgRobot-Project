using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeMovingForwardState : DrivingState<AutonomeMovingForwardState>
{

    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Drive(system, Direction.Forward, 0.25); // Move forward cautiously
        double currentDistance = drivingContext.ObstacleDetectionSystem.Distance;

        if (currentDistance < drivingContext.SafeDistanceThreshold)
        {
            system.SetState(AutonomeStoppingState.Instance); // Stop if obstacle is detected
        }
    }
}

