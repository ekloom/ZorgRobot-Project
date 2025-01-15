using System;

namespace RobotProject.ControlSystems.DrivingStrategySystem;

public interface IObjectDetectionState
{
    void Handle(DrivingSystem system);
}
