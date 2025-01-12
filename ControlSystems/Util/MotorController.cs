using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Util;

public abstract class MotorController : IUpdatable
{

  protected MotorMode _motorMode;


  protected short _targetSpeedL;
  protected short _targetSpeedR;
  protected short CurrentMotorSpeedL;
  protected short CurrentMotorSpeedR;

  private readonly Queue<MotorCommand> _commandQueue;
  private bool _IsCommandActive;

  public MotorController()
  {
    _commandQueue = new();
    _IsCommandActive = false;
  }

  private const int MaxQueueSize = 20;

  // Enqueues the command
  public void Drive(Direction direction, short speed)
  {
    if (_commandQueue.Count >= MaxQueueSize)
    {
      _commandQueue.Dequeue(); // Discard the oldest command
    }
    _commandQueue.Enqueue(new MotorCommand(direction, speed));
    Console.WriteLine("CommandQueue Size: {0}", _commandQueue.Count);
  }

  public void ResetMotors()
  {
    _commandQueue.Clear();
    _IsCommandActive = false;
  }


  private void ExecuteDrive(Direction direction, short Speed)
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
      rightSpeed += Speed;
      leftSpeed -= Speed;
    }

    if (direction.HasFlag(Direction.Left))
    {
      rightSpeed -= Speed;
      leftSpeed += Speed;
    }

    _targetSpeedL = leftSpeed;
    _targetSpeedR = rightSpeed;

    _motorMode = MotorMode.Run;
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

    if (CurrentMotorSpeedL == targetSpeedL && CurrentMotorSpeedR == targetSpeedR) _IsCommandActive = false;
  }


  public void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    try
    {
      Robot.Motors(SpeedL, SpeedR);
    }
    catch (IOException ex)
    {
      Console.WriteLine($"I2C communication error: {ex.Message}");
      // Optional: implement a retry mechanism
      Robot.Wait(100); // Add a small delay before retrying
    }

  }
  public virtual void Update()
  {

    if (!_IsCommandActive && _commandQueue.Count > 0)
    {
      var command = _commandQueue.Dequeue();
      ExecuteDrive(command.Direction, command.Speed);
      _IsCommandActive = true;
    }

    // Existing switch logic here...
    switch (_motorMode)
    {
      case MotorMode.stop:
        if (CurrentMotorSpeedL != 0 || CurrentMotorSpeedR != 0)
        {
          GradualDrive(0, 0, 5);
        }
        break;

      case MotorMode.Run:
        if (CurrentMotorSpeedL != _targetSpeedL || CurrentMotorSpeedR != _targetSpeedR)
        {
          GradualDrive(_targetSpeedL, _targetSpeedR, 5);
        }
        break;
    }

    Robot.Wait(50);
  }

}

public class MotorCommand
{
  public Direction Direction { get; }
  public short Speed { get; }

  public MotorCommand(Direction direction, short speed)
  {
    Direction = direction;
    Speed = speed;
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