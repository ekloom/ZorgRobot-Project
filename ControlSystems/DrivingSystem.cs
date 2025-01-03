using Avans.StatisticalRobot;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems;

internal class DrivingSystem : IUpdatable
{
    private readonly ObstacleDetectionSystem obstacleDetectionSystem;
    private readonly LCD16x2 _lcd;

    private bool isEmergencyStop;
    private MotorMode _motorMode;
    private short CurrentMotorSpeedL;
    private short CurrentMotorSpeedR;
    private short _targetSpeedL;
    private short _targetSpeedR;

    public bool IsFollowingTarget { get; set; }

    public DrivingSystem(LCD16x2 lcd, ObstacleDetectionSystem _obstacleDetectionSystem)
    {
        _motorMode = MotorMode.stop;
        _lcd = lcd;
        obstacleDetectionSystem = _obstacleDetectionSystem;
        MotorFuntions.SetMotorSpeed(0, 0);
    }

    public void Reset()
    {
        _motorMode = MotorMode.stop;
        isEmergencyStop = false;
        IsFollowingTarget = false;
        CurrentMotorSpeedL = 0;
        CurrentMotorSpeedR = 0;
    }

    public void Stop()
    {
        _motorMode = MotorMode.stop;
        _lcd.SetText("Stopped!");
        System.Console.WriteLine("Motor stopped!");
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true;
        _motorMode = MotorMode.stop;
        MotorFuntions.SetMotorSpeed(0, 0);
        System.Console.WriteLine("EmergencyStop activated!");
    }


    public void Drive(Direction direction, short Speed)
    {

        short leftSpeed = 0;
        short rightSpeed = 0;

        if (direction.HasFlag(Direction.Forward))
        {
            leftSpeed += Speed;
            rightSpeed += Speed;
        }

        if (direction.HasFlag(Direction.Backwards))
        {
            leftSpeed -= Speed;
            rightSpeed -= Speed;
        }

        if (direction.HasFlag(Direction.Right))
        {
            leftSpeed -= (short)(Speed / 2);
            rightSpeed += (short)(Speed / 2);
        }

        if (direction.HasFlag(Direction.Left))
        {
            leftSpeed += (short)(Speed / 2);
            rightSpeed -= (short)(Speed / 2);
        }

        _targetSpeedL = leftSpeed;
        _targetSpeedR = rightSpeed;

        _motorMode = MotorMode.Run;
    }

    void FollowTarget()
    {
        if (!obstacleDetectionSystem.IsPathClear())
        {
            Stop();
            Console.WriteLine("Obstacle detected!");
            _lcd.SetText("Obstacle detected!");
        }
        else
        {
            Drive(Direction.Forward, 100);
            Console.WriteLine("Following target...");
            _lcd.SetText("Following target...");
        }
    }


    public void Update()
    {
        // Handle periodic updates if necessary
        if (!isEmergencyStop)
        {
            if (IsFollowingTarget)
            {
                FollowTarget();
            }

            switch (_motorMode)
            {
                case MotorMode.stop:
                    if (CurrentMotorSpeedL != 0 && CurrentMotorSpeedR != 0)
                    {
                        MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, 0, 0);
                    }
                    break;
                case MotorMode.Run:
                    if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
                    {
                        MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, _targetSpeedL, _targetSpeedR);
                    }
                    break;

            }
        }
    }

}


[Flags]
public enum Direction
{
    Forward = 1,
    Backwards = 2,
    Left = 4,
    Right = 5
}

public enum MotorMode
{
    stop = 0,
    Run,
}
