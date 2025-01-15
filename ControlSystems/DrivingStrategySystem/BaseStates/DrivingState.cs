using System;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems.DrivingStrategySystem.BaseStates;

public abstract class DrivingState<T> : IDrivingState where T : DrivingState<T>, new()
{
    private static T _instance;

    public static T Instance => _instance ??= new T(); // Singleton instance

    public abstract void Handle(DrivingSystem system, DrivingContext context);

    protected void Stop(DrivingSystem system)
    {
        system.Stop();
    }

    protected void Drive(DrivingSystem system, Direction direction, double speed)
    {
        system.Drive(direction, speed);
    }
}

