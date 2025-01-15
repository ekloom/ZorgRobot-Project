using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;

public class AutonomeCheckingDistanceState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public AutonomeCheckingDistanceState(DrivingContext context)
    {
        _context = context;
    }

    public override void Handle(DrivingSystem system)
    {
        Stop(system); // Ensure the robot is stationary
        double currentDistance = _context.ObstacleDetectionSystem.Distance;

        if (currentDistance < _context.SafeDistanceThreshold)
        {
            system.SetState(new AutonomeStoppingState(_context)); // Too close, stop
            _context.LoggingSystem.LogToLcd("obstacle detected...");
        }
        else
        {
            Stop(system);
            system.SetState(new AutonomeMovingForwardState(_context)); // Safe to move forward
        }
    }
}

