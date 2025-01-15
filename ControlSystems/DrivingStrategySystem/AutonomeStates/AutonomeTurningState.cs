using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeTurningState : DrivingState<AutonomeTurningState>
{
    public override void Handle(DrivingSystem system, DrivingContext drivingContext)
    {
        Drive(system, Direction.Right, 0.15); // Start turning

        if (drivingContext.HasTurnedEnough())
        {
            system.SetState(AutonomeCheckingDistanceState.Instance); // Re-check distance
            Stop(system);
        }
    }
}

