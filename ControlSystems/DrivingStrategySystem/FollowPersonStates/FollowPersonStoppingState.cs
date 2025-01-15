using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonStoppingState : DrivingState
{
    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public FollowPersonStoppingState(DrivingContext context)
    {
        _context = context;
    }
    public override void Handle(DrivingSystem system)
    {
        Stop(system);

        if (system.CurrentMotorSpeedL == 0 && system.CurrentMotorSpeedR == 0)
        {
            system.SetState(new FollowPersonCheckingDistanceState(_context)); // Check for movement again
        }
    }
}

