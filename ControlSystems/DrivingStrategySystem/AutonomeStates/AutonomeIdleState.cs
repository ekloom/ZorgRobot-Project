using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeIdleState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public AutonomeIdleState(DrivingContext context)
    {
        _context = context;
    }
    public override void Handle(DrivingSystem system)
    {

        // Remain in idle until another state triggers a transition
        if (_context.PIRMotion.Watch() == 1)
        {
            system.SetState(new AutonomeMovingForwardState(_context));
        }
        else
        {
            Stop(system); // Stop the robot
        }

    }
}
