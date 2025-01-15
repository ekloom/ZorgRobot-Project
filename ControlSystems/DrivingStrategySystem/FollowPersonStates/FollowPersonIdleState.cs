using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonIdleState : DrivingState<FollowPersonIdleState>
{
    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {

        if (drivingContext.PIRMotion.Watch() == 1)
        {
            system.SetState(FollowPersonCheckingDistanceState.Instance);
        }
        else
        {
            Stop(system);
        }


        // Stay idle until movement is detected
    }
}

