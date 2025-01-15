using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonMovingForwardState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public FollowPersonMovingForwardState(DrivingContext context)
    {
        _context = context;
    }

    public override void Handle(DrivingSystem system)
    {
        Drive(system, Direction.Forward, 0.5); // Move forward

        double currentDistance = _context.ObstacleDetectionSystem.Distance;
        if (!_context.IsDistanceChanging(currentDistance, _context.PreviousDistance))
        {
            system.SetState(new FollowPersonStoppingState(_context)); // Pass the context to next state
        }

        _context.PreviousDistance = currentDistance; // Update previous distance
    }
}

