using System;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

public abstract class DrivingState : IObjectDetectionState
{
    public abstract void Handle(DrivingSystem system);

    protected void Stop(DrivingSystem system)
    {
        system.Stop();
    }
    protected void Drive(DrivingSystem system, Direction direction, double speed)
    {
        system.Drive(direction, speed);
    }
}
