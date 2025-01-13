using Avans.StatisticalRobot;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems;

internal class DrivingSystem : MotorController
{
    private readonly ObstacleDetectionSystem _obstacleDetectionSystem;
    private readonly LoggingSystem _loggingSystem;
    private bool isEmergencyStop;

    private bool _isScanning;
    private int _scanstep = 20;
    private float _lastDetectedDistance = 0;
    private int _targetAngle = 0;
    private int _totalRotation = 0;

    private int _maxDistanceFromPerson = 250;
    private int _minDistanceFromPerson = 50;

    public DrivingMode DrivingMode { get; set; }
    public bool IsPersonFound { get; private set; }
    public bool HasPerformedScan { get; private set; }

    // private readonly PIRMotion pIRMotion;

    static DateTime lastScanTime;

    public DrivingSystem(ObstacleDetectionSystem obstacleDetectionSystem, LoggingSystem loggingSystem)
    {
        _motorMode = MotorMode.stop;
        _obstacleDetectionSystem = obstacleDetectionSystem;
        _loggingSystem = loggingSystem;
        SetMotorSpeed(0, 0); // Set motor speed to 0
    }

    public void Reset()
    {
        _motorMode = MotorMode.stop; // Set the motor mode to stop
        isEmergencyStop = false; // Reset the emergency stop flag
        DrivingMode = DrivingMode.Autonome; // Set the driving mode to Autonome
        CurrentMotorSpeedL = 0; // Set 'CurrentMotorSpeedL' to 0
        CurrentMotorSpeedR = 0; // Set 'CurrentMotorSpeedR' to 0
    }

    public void Stop()
    {
        _motorMode = MotorMode.stop;
        System.Console.WriteLine("Motor stopped!");
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true; // set the emergency stop flag to true
        _motorMode = MotorMode.stop; // Set the motor mode to stop
        SetMotorSpeed(0, 0); // stop motors
        ResetMotors();
        System.Console.WriteLine("EmergencyStop activated!");
    }

    public void DriveAutonome()
    {

    }

    public void StartScanning()
    {
        _totalRotation = 0; // Reset the total rotation counter
        _scanstep = 45; // Setting the scan step to 45 degrees
        _isScanning = true; // Setting the scanning flag to true
        HasPerformedScan = false; // Reset the scan flag
        lastScanTime = DateTime.MinValue; // Reset the last scan time
        _lastDetectedDistance = float.MaxValue; // Reset the last detected distance
        DrivingMode = DrivingMode.FollowPerson; // Set the driving mode to follow person
        Stop();
    }

    void PerformScanning()
    {
        if (!_isScanning) return;

        const int waitTimeMs = 1000; // 1 second wait

        _totalRotation += _scanstep;

        // Check if enough time has passed
        if ((DateTime.Now - lastScanTime).TotalMilliseconds < waitTimeMs)
        {
            // Continue waiting
            return;
        }

        // Update the last scan time
        lastScanTime = DateTime.Now;

        // Gets the current distance to a osbtacle
        int currentDistance = _obstacleDetectionSystem.Distance;

        // Update the closest distance detected
        if (currentDistance < _lastDetectedDistance && currentDistance < _maxDistanceFromPerson)
        {
            // Start turning 45 degrees

            // Stop motion
            Stop();
        }

        // Stop scanning if a full rotation is completed or a close distance is detected
        if (_totalRotation >= 360 || (_lastDetectedDistance > _minDistanceFromPerson && _lastDetectedDistance < _maxDistanceFromPerson))
        {

        }
    }


    void FollowPerson()
    {
        int distance = _obstacleDetectionSystem.Distance;

        // Check if the person is within the updated range
        if (distance > _minDistanceFromPerson && distance < _maxDistanceFromPerson)
        {
            Drive(Direction.Forward, 50); // Keep following the person
            _loggingSystem.LogToLcd("Following person...");

            // Dynamically adjust the follow range as the person moves
            _minDistanceFromPerson = Math.Max((int)(distance * 0.8), 40); // Minimum distance is 80% of current
            _maxDistanceFromPerson = Math.Min((int)(distance * 1.5), 400); // Maximum distance is 150% of current

            Console.WriteLine("Updated Distances While Following - Min: {0}, Max: {1}", _minDistanceFromPerson, _maxDistanceFromPerson);
        }
        else
        {
            // Lost person, start scanning again
            _loggingSystem.LogToLcd("Lost person, starting scan...");
            StartScanning();
        }
    }


    public override void Update()
    {
        _lastDetectedDistance = _obstacleDetectionSystem.Distance;

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

            if (_isScanning)
            {
                PerformScanning();
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


