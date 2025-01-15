using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonCheckingDistanceState : DrivingState<FollowPersonCheckingDistanceState>
{


    private DateTime _lastStateChangeTime; // Track the time of the last state change

    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Stop(system); // Stop the motors

        double currentDistance = drivingContext.ObstacleDetectionSystem.Distance;
        Console.WriteLine("currentDistance: {0}, PreviousDistance: {1}", currentDistance, drivingContext.PreviousDistance);

        // Check if enough time has passed since the last state change
        if ((DateTime.Now - _lastStateChangeTime).TotalMilliseconds >= 500) // Example: 500 ms grace period
        {
            if (drivingContext.IsDistanceChanging(currentDistance, drivingContext.PreviousDistance))
            {
                system.SetState(FollowPersonMovingForwardState.Instance); // Start following the person
            }
            else
            {
                system.SetState(FollowPersonStoppingState.Instance); // Stop and re-evaluate
            }

            _lastStateChangeTime = DateTime.Now; // Update the last state change time
        }

        drivingContext.PreviousDistance = currentDistance; // Update previous distance
    }
}

