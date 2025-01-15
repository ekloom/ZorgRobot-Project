using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeTurningState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public AutonomeTurningState(DrivingContext context)
    {
        _context = context;
    }
    public override void Handle(DrivingSystem system)
    {
        Drive(system, Direction.Right, 0.15); // Start turning

        if (_context.HasTurnedEnough())
        {
            system.SetState(new AutonomeCheckingDistanceState(_context)); // Re-check distance
            Stop(system);
        }
    }
}

