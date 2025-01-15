using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeMovingForwardState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public AutonomeMovingForwardState(DrivingContext context)
    {
        _context = context;
    }

    public override void Handle(DrivingSystem system)
    {
        Drive(system, Direction.Forward, 0.25); // Move forward cautiously
        double currentDistance = _context.ObstacleDetectionSystem.Distance;

        if (currentDistance < _context.SafeDistanceThreshold)
        {
            system.SetState(new AutonomeStoppingState(_context)); // Stop if obstacle is detected
        }
    }
}

