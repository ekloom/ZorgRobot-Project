using System;
using RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

namespace RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;

public class FollowPersonIdleState : DrivingState
{

    private readonly DrivingContext _context;

    // Constructor now takes a DrivingContext as the parameter
    public FollowPersonIdleState(DrivingContext context)
    {
        _context = context;
    }
    public override void Handle(DrivingSystem system)
    {
        //

        if (_context.PIRMotion.Watch() == 1)
        {
            system.SetState(new FollowPersonCheckingDistanceState(_context));
        }
        else
        {
            Stop(system);
        }


        // Stay idle until movement is detected
    }
}

