using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeStoppingState : DrivingState<AutonomeStoppingState>
{

    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Stop(system);

        if (system.CurrentMotorSpeedL == 0 && system.CurrentMotorSpeedR == 0) // Ensure motors are stopped
        {
            system.SetState(AutonomeTurningState.Instance); // Start turning
        }
    }
}
