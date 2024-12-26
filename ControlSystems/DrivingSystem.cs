using Avans.StatisticalRobot;
using RobotProject.Util;

namespace RobotProject.ControlSystems;

public class DrivingSystem : IUpdatable
{


    private readonly ObstacleDetectionSystem obstacleDetectionSystem;

    private bool isEmergencyStop;

    private short CurrentMotorSpeedL;
    private short CurrentMotorSpeedR;

    private MotorMode _motorMode;

    private readonly LCD16x2 _lcd;

    short _targetSpeedR;
    short _targetSpeedL;

    public DrivingSystem(LCD16x2 lcd)
    {
        _motorMode = MotorMode.stop;
        _lcd = lcd;
        obstacleDetectionSystem = new ObstacleDetectionSystem(16);
    }


    public void SetTargetSpeed(MotorMode motorMode, short targetSpeedR, short targetSpeedL)
    {
        _targetSpeedR = targetSpeedR;
        _targetSpeedL = targetSpeedL;
        _motorMode = motorMode;
    }

    public void Stop()
    {
        _motorMode = MotorMode.stop;
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true;
        _motorMode = MotorMode.stop;
        MotorFuntions.SetMotorSpeed(0, 0);
    }

    public void FollowTarget()
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


    public void Update()
    {
        // Handle periodic updates if necessary
        if (!isEmergencyStop)
        {

            switch (_motorMode)
            {
                case MotorMode.stop:
                    MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, 0, 0);
                    break;
                case MotorMode.Run:
                    if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
                    {
                        MotorFuntions.EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, _targetSpeedL, _targetSpeedR);
                    }
                    else
                    {
                        MotorFuntions.SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);
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

public enum AutoMode
{
    FollowTarget,
}