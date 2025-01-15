using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonStoppingState : DrivingState<FollowPersonStoppingState>
{
    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Stop(system);

        if (system.CurrentMotorSpeedL == 0 && system.CurrentMotorSpeedR == 0)
        {
            system.SetState(FollowPersonCheckingDistanceState.Instance); // Check for movement again
        }
    }
}

