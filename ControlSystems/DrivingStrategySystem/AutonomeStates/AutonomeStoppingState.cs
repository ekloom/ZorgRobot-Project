using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeStoppingState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public AutonomeStoppingState(DrivingContext context)
    {
        _context = context;
    }
    public override void Handle(DrivingSystem system)
    {
        Stop(system);

        if (system.CurrentMotorSpeedL == 0 && system.CurrentMotorSpeedR == 0) // Ensure motors are stopped
        {
            system.SetState(new AutonomeTurningState(_context)); // Start turning
        }
    }
}
