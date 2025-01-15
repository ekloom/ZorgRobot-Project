using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeIdleState : DrivingState<AutonomeIdleState>
{

    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {

        // Remain in idle until another state triggers a transition
        // if (drivingContext.PIRMotion.Watch() == 1)
        // {
        //     system.SetState(AutonomeIdleState.Instance);
        // }
        // else
        // {
        //     Stop(system); // Stop the robot
        // }

        system.SetState(AutonomeMovingForwardState.Instance);

    }
}
