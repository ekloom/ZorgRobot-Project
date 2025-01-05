using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Util;

public abstract class MotorController : IUpdatable
{

  protected MotorMode _motorMode;
  protected short _targetSpeedL;
  protected short _targetSpeedR;
  protected short CurrentMotorSpeedL;
  protected short CurrentMotorSpeedR;

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
      // only if forward and right angles is requested the same time
      // rightSpeed += (short)(Speed / 2);
      rightSpeed += Speed;
    }

    if (direction.HasFlag(Direction.Left))
    {
      // leftSpeed += (short)(Speed / 2);
      leftSpeed += Speed;
    }

    _targetSpeedL = leftSpeed;
    _targetSpeedR = rightSpeed;

    _motorMode = MotorMode.Run;
  }
  public void EaseOutMotors(ref short currentMotorSpeedL, ref short currentMotorSpeedR, short targetSpeedLeft, short targetSpeedRight)
  {
    const float stepTimeMs = 50.0f; // Time per step in milliseconds
    const float durationMs = 3000.0f; // Total duration for ease out (adjust as needed)
    float elapsedTime = 0.0f;

    while (elapsedTime <= durationMs)
    {
      // Calculate the progress normalized to [0, 1]
      float t = elapsedTime / durationMs;

      // Apply the easing function
      float easedT = MathFunctions.EaseOutCubic(t);

      // Interpolate motor speeds
      currentMotorSpeedL = (short)(currentMotorSpeedL + (targetSpeedLeft - currentMotorSpeedL) * easedT);
      currentMotorSpeedR = (short)(currentMotorSpeedR + (targetSpeedRight - currentMotorSpeedR) * easedT);

      // Update motor speed
      SetMotorSpeed(currentMotorSpeedL, currentMotorSpeedR);
      Console.WriteLine("Motor L: {0}, Motor R: {1}", currentMotorSpeedL, currentMotorSpeedR);

      // Break the loop if speeds are close enough to target
      if (Math.Abs(currentMotorSpeedL - targetSpeedLeft) <= 1 && Math.Abs(currentMotorSpeedR - targetSpeedRight) <= 1)
      {
        break;
      }

      // Wait and increment time
      Robot.Wait((int)stepTimeMs / 2);
      elapsedTime += stepTimeMs;
    }

    // Ensure the final speeds are set to target
    currentMotorSpeedL = targetSpeedLeft;
    currentMotorSpeedR = targetSpeedRight;
    SetMotorSpeed(targetSpeedLeft, targetSpeedRight);
  }

  public void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    // Apply speed ti the motors
    Robot.Motors(SpeedL, SpeedR);
  }
  public virtual void Update()
  {
    switch (_motorMode)
    {
      case MotorMode.stop:
        if (CurrentMotorSpeedL != 0 && CurrentMotorSpeedR != 0)
        {
          EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, 0, 0);
        }
        break;
      case MotorMode.Run:
        if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
        {
          EaseOutMotors(ref CurrentMotorSpeedL, ref CurrentMotorSpeedR, _targetSpeedL, _targetSpeedR);
        }
        break;

    }

  }
}

public enum MotorMode
{
  stop = 0,
  Run,
}

[Flags]
public enum Direction
{
  Forward = 1,
  Backwards = 2,
  Left = 4,
  Right = 5
}