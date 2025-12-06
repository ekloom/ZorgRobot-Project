using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Util;

public abstract class MotorController : IUpdatable
{

  private MotorMode _motorMode;

  internal short CurrentMotorSpeedL { get; private set; }

  internal short CurrentMotorSpeedR { get; private set; }

  internal short TargetSpeedL { get; private set; }

  internal short TargetSpeedR { get; private set; }


  /// <summary>
  /// This will enqeue the drive method
  /// </summary>
  /// <param name="speed">0.0 = stop, -1.0 = full speed reverse, 1.0 = full speed forward</param>
  /// <param name="direction"></param>
  public void Drive(Direction direction, double speed)
  {
    ApplyMotorSettings(new MotorSettings(direction, speed));
  }

  public void Stop()
  {
    ApplyMotorSettings(new MotorSettings(Direction.None, 0));
  }

  public void ResetMotors()
  {
    _motorMode = MotorMode.stop;
    CurrentMotorSpeedL = 0; // Set 'CurrentMotorSpeedL' to 0
    CurrentMotorSpeedR = 0; // Set 'CurrentMotorSpeedR' to 0
  }


  private void ApplyMotorSettings(MotorSettings motorSettings)
  {

    short leftSpeed = 0;
    short rightSpeed = 0;

    short speedConvertion = (short)Math.Round(motorSettings.Speed * 300.0);

    if (motorSettings.Direction == Direction.Forward)
    {
      leftSpeed += speedConvertion;
      rightSpeed += speedConvertion;
    }

    if (motorSettings.Direction == Direction.Backwards)
    {
      leftSpeed -= speedConvertion;
      rightSpeed -= speedConvertion;
    }

    if (motorSettings.Direction == Direction.Right)
    {
      rightSpeed += speedConvertion;
      leftSpeed -= (short)(speedConvertion / 2);
    }

    if (motorSettings.Direction == Direction.Left)
    {
      leftSpeed += speedConvertion;
      rightSpeed -= (short)(speedConvertion / 2);
    }

    TargetSpeedL = leftSpeed;
    TargetSpeedR = rightSpeed;

    _motorMode = motorSettings.MotorMode;
  }

  private void GradualDrive(short targetSpeedL, short targetSpeedR, short step)
  {
    // Apply the speeds to the motors
    SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);

    // Adjust the left motor speed
    if (CurrentMotorSpeedL < targetSpeedL)
    {
      CurrentMotorSpeedL += step;
      if (CurrentMotorSpeedL > targetSpeedL) CurrentMotorSpeedL = targetSpeedL;
    }
    else if (CurrentMotorSpeedL > targetSpeedL)
    {
      CurrentMotorSpeedL -= step;
      if (CurrentMotorSpeedL < targetSpeedL) CurrentMotorSpeedL = targetSpeedL;
    }

    // Adjust the right motor speed
    if (CurrentMotorSpeedR < targetSpeedR)
    {
      CurrentMotorSpeedR += step;
      if (CurrentMotorSpeedR > targetSpeedR) CurrentMotorSpeedR = targetSpeedR;
    }
    else if (CurrentMotorSpeedR > targetSpeedR)
    {
      CurrentMotorSpeedR -= step;
      if (CurrentMotorSpeedR < targetSpeedR) CurrentMotorSpeedR = targetSpeedR;
    }

    Console.WriteLine("Current speed left motor: {0}, Current speed right motor: {1}", CurrentMotorSpeedL, CurrentMotorSpeedR);
  }

  public void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    try
    {
      Robot.Motors(SpeedL, SpeedR);
    }
    catch (IOException ex)
    {
      Console.WriteLine($"Attempting to set motor speeds: Left={SpeedL}, Right={SpeedR}");
      Robot.Wait(100); // Add a small delay before retrying
    }

  }
  public virtual void Update()
  {

    switch (_motorMode)
    {
      case MotorMode.stop:
        GradualDrive(0, 0, 25);
        break;

      case MotorMode.Run:
        GradualDrive(TargetSpeedL, TargetSpeedR, 5);
        break;
    }

    Robot.Wait(100);
  }

}

public class MotorSettings
{
  public Direction Direction { get; }
  public MotorMode MotorMode { get; }

  private double _Speed;
  public double Speed
  {
    get
    {
      return _Speed;
    }
    set
    {
      if (value <= -1.0)
      {
        value = -1.0;
      }
      else if (value >= 1.0)
      {
        value = 1.0;
      }

      _Speed = value;
    }
  }

  public MotorSettings(Direction direction, double speed)
  {
    Direction = direction;
    Speed = speed;
    MotorMode = direction == Direction.None ? MotorMode.stop : MotorMode.Run;
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
  None = 0,
  Forward = 1,
  Backwards = 2,
  Left = 4,
  Right = 5
}