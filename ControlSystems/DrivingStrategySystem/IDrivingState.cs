using System;

namespace RobotProject.ControlSystems.DrivingStrategySystem;

public interface IDrivingState
{
    void Handle(DrivingSystem system, DrivingContext context);
}
