using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeCheckingDistanceState : DrivingState<AutonomeCheckingDistanceState>
{

    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Stop(system); // Ensure the robot is stationary
        double currentDistance = drivingContext.ObstacleDetectionSystem.Distance;

        if (currentDistance < drivingContext.SafeDistanceThreshold)
        {
            system.SetState(AutonomeStoppingState.Instance); // Too close, stop
            drivingContext.LoggingSystem.LogToLcd("obstacle detected...");
        }
        else
        {
            Stop(system);
            system.SetState(AutonomeMovingForwardState.Instance); // Safe to move forward
        }
    }
}

