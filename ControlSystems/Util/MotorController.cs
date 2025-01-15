using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Util;

public abstract class MotorController : IUpdatable
{

  private MotorMode _motorMode;


  protected short _targetSpeedL;
  protected short _targetSpeedR;
  private short CurrentMotorSpeedL;
  private short CurrentMotorSpeedR;

  private readonly Queue<MotorCommand> _commandQueue;

  private bool _IsCommandActive;

  public MotorController()
  {
    _commandQueue = new();
    _IsCommandActive = false;
  }

  /// <summary>
  /// This will enqeue the drive method
  /// </summary>
  /// <param name="speed">0.0 = stop, -1.0 = full speed reverse, 1.0 = full speed forward</param>
  /// <param name="direction"></param>
  public void Drive(double speed, Direction direction)
  {

    if (_commandQueue.Count > 5) return;


    // Avoid adding duplicates
    if (_commandQueue.Count > 0)
    {
      var lastCommand = _commandQueue.Last();
      if (lastCommand.Speed == speed && lastCommand.Direction == direction)
      {
        // Adjust the last command instead of adding a new one
        lastCommand.Speed = speed;

        // Skip adding the new command if it's identical to the last one
        Console.WriteLine("Duplicate command ignored.");
        return;
      }
    }

    var command = new MotorCommand(speed, direction);
    _commandQueue.Enqueue(command);
    Console.WriteLine("CommandQueue Size: {0}", _commandQueue.Count);

  }

  public void ResetMotors()
  {
    _commandQueue.Clear();
    _IsCommandActive = false;
    _motorMode = MotorMode.stop;
    CurrentMotorSpeedL = 0; // Set 'CurrentMotorSpeedL' to 0
    CurrentMotorSpeedR = 0; // Set 'CurrentMotorSpeedR' to 0
  }


  private void SetMotorSettings(double Speed, Direction direction, MotorMode motorMode)
  {

    short leftSpeed = 0;
    short rightSpeed = 0;

    short speedConvertion = (short)Math.Round(Speed * 300.0);

    if (direction.HasFlag(Direction.Forward))
    {
      leftSpeed += speedConvertion;
      rightSpeed += speedConvertion;
    }

    if (direction.HasFlag(Direction.Backwards))
    {
      leftSpeed -= speedConvertion;
      rightSpeed -= speedConvertion;
    }

    if (direction.HasFlag(Direction.Right))
    {
      rightSpeed += speedConvertion;
      leftSpeed -= (short)(speedConvertion / 2);
    }

    if (direction.HasFlag(Direction.Left))
    {
      leftSpeed += speedConvertion;
      rightSpeed -= (short)(speedConvertion / 2);
    }

    _targetSpeedL = leftSpeed;
    _targetSpeedR = rightSpeed;

    _motorMode = motorMode;
  }

  void GradualDrive(short targetSpeedL, short targetSpeedR, short step)
  {
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

    // Apply the speeds to the motors
    SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);

  }


  public void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    try
    {
      Robot.Motors(SpeedL, SpeedR);
      if (CurrentMotorSpeedL == _targetSpeedL && CurrentMotorSpeedR == _targetSpeedR) _IsCommandActive = false;
    }
    catch (IOException ex)
    {
      Console.WriteLine($"I2C communication error: {ex.Message}");
      Robot.Wait(100); // Add a small delay before retrying
    }

  }
  public virtual void Update()
  {

    if (!_IsCommandActive && _commandQueue.Count > 0)
    {
      var command = _commandQueue.Dequeue();
      SetMotorSettings(command.Speed, command.Direction, command.MotorMode);
      _IsCommandActive = true;
    }

    // Existing switch logic here...
    switch (_motorMode)
    {
      case MotorMode.stop:
        // if (CurrentMotorSpeedL != 0 || CurrentMotorSpeedR != 0)
        // {
        //   GradualDrive(0, 0, 2);
        // }

        GradualDrive(0, 0, 5);
        break;

      case MotorMode.Run:
        // if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
        // {
        //   GradualDrive(_targetSpeedL, _targetSpeedR, 2);
        // }

        GradualDrive(_targetSpeedL, _targetSpeedR, 5);
        break;
    }

    Robot.Wait(50);
  }

}

public class MotorCommand
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

  public MotorCommand(double speed, Direction direction)
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