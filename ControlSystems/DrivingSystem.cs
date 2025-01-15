using Avans.StatisticalRobot;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems;

internal class DrivingSystem : MotorController
{
    private readonly ObstacleDetectionSystem _obstacleDetectionSystem;
    private readonly LoggingSystem _loggingSystem;
    private bool isEmergencyStop;

    private ObjectdectionState _currentState;
    private double _previousDistance;
    private double _currentDistance;


    public DrivingMode DrivingMode { get; set; }
    public bool IsPersonFound { get; private set; }
    public bool HasPerformedScan { get; private set; }

    int SafeDistanceThreshold = 20;

    // private readonly PIRMotion pIRMotion;

    static DateTime lastScanTime;

    public DrivingSystem(ObstacleDetectionSystem obstacleDetectionSystem, LoggingSystem loggingSystem)
    {
        ResetMotors();
        _obstacleDetectionSystem = obstacleDetectionSystem;
        _loggingSystem = loggingSystem;
        SetMotorSpeed(0, 0); // Set motor speed to 0
        DrivingMode = DrivingMode.Idle;
        _currentState = ObjectdectionState.Idle;
    }

    public void Reset()
    {
        ResetMotors();
        isEmergencyStop = false; // Reset the emergency stop flag
        DrivingMode = DrivingMode.Idle;
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true; // set the emergency stop flag to true
        SetMotorSpeed(0, 0); // stops the motors
        ResetMotors();
        System.Console.WriteLine("EmergencyStop activated!");
    }


    bool HasTurnedEnough()
    {
        // Check the current distance from the ultrasonic sensor
        double frontDistance = _obstacleDetectionSystem.Distance;

        // Return true if the front distance is above a safe threshold
        return frontDistance > SafeDistanceThreshold;
    }


    private bool IsDistanceChanging(double currentDistance, double previousDistance)
    {
        return currentDistance > previousDistance || currentDistance < previousDistance;
    }


    void DriveAutonome()
    {
        switch (_currentState)
        {
            case ObjectdectionState.Idle:
                // The robot is idle, do nothing
                Stop();
                break;

            case ObjectdectionState.CheckingDistance:
                Stop(); // Ensure the robot is stationary
                _currentDistance = _obstacleDetectionSystem.Distance; // Get the current distance to the nearest obstacle
                if (_currentDistance < SafeDistanceThreshold) // If too close to an obstacle
                {
                    _currentState = ObjectdectionState.Stopping; // Transition to stopping
                }
                else
                {
                    _currentState = ObjectdectionState.MovingForward; // Safe to move forward
                }
                break;

            case ObjectdectionState.Stopping:
                Stop(); // Stop all motion
                if (CurrentMotorSpeedL == 0 && CurrentMotorSpeedR == 0) // Ensure the motors are stopped
                {
                    _currentState = ObjectdectionState.Turning; // Transition to turning
                }
                break;

            case ObjectdectionState.Turning:
                Drive(Direction.Right, 0.5); // Turn to avoid the obstacle
                if (HasTurnedEnough()) // A method to check if the robot has turned sufficiently
                {
                    _currentState = ObjectdectionState.CheckingDistance; // Re-check the distance after turning
                }
                break;

            case ObjectdectionState.MovingForward:
                Drive(Direction.Forward, 0.5); // Move forward cautiously
                _currentDistance = _obstacleDetectionSystem.Distance; // Continuously check distance
                if (_currentDistance < SafeDistanceThreshold) // Obstacle detected
                {
                    _currentState = ObjectdectionState.Stopping; // Transition to stopping
                }
                break;
        }
    }
    void FollowPerson()
    {
        switch (_currentState)
        {
            case ObjectdectionState.Stopping:
                Stop();
                if (CurrentMotorSpeedL == 0 && CurrentMotorSpeedR == 0)
                {
                    _currentState = ObjectdectionState.Turning; // Transition to turning
                }
                break;

            case ObjectdectionState.Turning:
                Drive(Direction.Right, 0.5); // Start turning
                if (CurrentMotorSpeedL == TargetSpeedL && CurrentMotorSpeedR == TargetSpeedR)
                {
                    _currentState = ObjectdectionState.CheckingDistance; // Transition to checking distance
                }
                break;

            case ObjectdectionState.CheckingDistance:
                Stop(); // Stop the motors
                _currentDistance = _obstacleDetectionSystem.Distance; // Check distance
                if (IsDistanceChanging(_currentDistance, _previousDistance))
                {
                    _currentState = ObjectdectionState.MovingForward; // Transition to moving forward
                }
                else
                {
                    _currentState = ObjectdectionState.Stopping; // Repeat the process
                }
                _previousDistance = _currentDistance;
                break;

            case ObjectdectionState.MovingForward:
                Drive(Direction.Forward, 0.5); // Move forward
                _currentDistance = _obstacleDetectionSystem.Distance; // Check distance
                if (!IsDistanceChanging(_currentDistance, _previousDistance))
                {
                    _currentState = ObjectdectionState.Stopping; // Stop and re-evaluate
                }
                _previousDistance = _currentDistance;
                break;

            case ObjectdectionState.Idle:
                Stop();
                break;
        }
    }


    public override void Update()
    {

        if (!isEmergencyStop)
        {

            switch (DrivingMode)
            {
                case DrivingMode.Idle:
                    Stop();
                    break;
                case DrivingMode.Autonome:
                    DriveAutonome();
                    break;
                case DrivingMode.FollowPerson:
                    FollowPerson();
                    break;
            }

            base.Update();
        }
    }

}

public enum DrivingMode
{
    Idle,
    Autonome,
    FollowPerson
}

public enum ObjectdectionState
{
    Stopping,
    Turning,
    CheckingDistance,
    MovingForward,
    Idle
}

