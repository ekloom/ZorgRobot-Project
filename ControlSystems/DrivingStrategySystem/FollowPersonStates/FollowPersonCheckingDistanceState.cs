using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonCheckingDistanceState : DrivingState
{

    private readonly DrivingContext _context;
    private DateTime _lastStateChangeTime; // Track the time of the last state change

    public FollowPersonCheckingDistanceState(DrivingContext context)
    {
        _context = context;
        _lastStateChangeTime = DateTime.Now; // Initialize with the current time
    }

    public override void Handle(DrivingSystem system)
    {
        Stop(system); // Stop the motors

        double currentDistance = _context.ObstacleDetectionSystem.Distance;
        Console.WriteLine("currentDistance: {0}, PreviousDistance: {1}", currentDistance, _context.PreviousDistance);

        // Check if enough time has passed since the last state change
        if ((DateTime.Now - _lastStateChangeTime).TotalMilliseconds >= 500) // Example: 500 ms grace period
        {
            if (_context.IsDistanceChanging(currentDistance, _context.PreviousDistance))
            {
                system.SetState(new FollowPersonMovingForwardState(_context)); // Start following the person
            }
            else
            {
                system.SetState(new FollowPersonStoppingState(_context)); // Stop and re-evaluate
            }

            _lastStateChangeTime = DateTime.Now; // Update the last state change time
        }

        _context.PreviousDistance = currentDistance; // Update previous distance
    }
}

