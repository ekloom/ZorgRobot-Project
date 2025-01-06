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

    public bool IsFollowingPerson { get; set; }
    public bool IsPersonFound { get; private set; }
    public bool HasPerformedScan { get; private set; }

    public DrivingSystem(ObstacleDetectionSystem obstacleDetectionSystem, LoggingSystem loggingSystem)
    {
        _motorMode = MotorMode.stop;
        _obstacleDetectionSystem = obstacleDetectionSystem;
        _loggingSystem = loggingSystem;
        SetMotorSpeed(0, 0);
    }

    public void Reset()
    {
        _motorMode = MotorMode.stop;
        isEmergencyStop = false;
        IsFollowingPerson = false;
        CurrentMotorSpeedL = 0;
        CurrentMotorSpeedR = 0;
    }

    public void Stop()
    {
        _motorMode = MotorMode.stop;
        System.Console.WriteLine("Motor stopped!");
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true;
        _motorMode = MotorMode.stop;
        SetMotorSpeed(0, 0);
        System.Console.WriteLine("EmergencyStop activated!");
    }

    public void StartScanning()
    {
        _totalRotation = 0;
        _scanstep = 30;
        _isScanning = true;
        HasPerformedScan = false;
        _lastDetectedDistance = float.MaxValue;
        IsFollowingPerson = false;
        Stop();
    }

    void PerformScanning()
    {
        if (!_isScanning) return;

        _totalRotation += _scanstep;

        int currentDistance = _obstacleDetectionSystem.Distance;

        // Update the closest distance detected
        if (currentDistance < _lastDetectedDistance && currentDistance < _maxDistanceFromPerson)
        {
            _lastDetectedDistance = currentDistance;
            _targetAngle = _totalRotation;
        }

        // Adjust rotation towards the target angle
        int adjustments = _targetAngle - _totalRotation;

        if (Math.Abs(adjustments) > _scanstep)
        {
            Direction dir = adjustments > 0 ? Direction.Right : Direction.Left;
            EnqueDrive(dir, (short)Math.Min(Math.Abs(adjustments), 50));
        }

        Console.WriteLine("Scanning - TotalRotation: {0}, TargetAngle: {1}, Distance: {2}", _totalRotation, _targetAngle, _lastDetectedDistance);

        // Stop scanning if a full rotation or close distance is detected
        if (_totalRotation >= 360 || _lastDetectedDistance < _maxDistanceFromPerson / 2)
        {
            _isScanning = false; // Stop scanning

            if (_lastDetectedDistance > _minDistanceFromPerson && _lastDetectedDistance < _maxDistanceFromPerson)
            {
                EnqueDrive(Direction.Forward, 50); // Drive forward towards the person

                // Dynamically adjust the minimum and maximum distances based on last detection
                _minDistanceFromPerson = Math.Max((int)(_lastDetectedDistance * 0.8), 20); // Minimum distance is 80% of last detected
                _maxDistanceFromPerson = Math.Min((int)(_lastDetectedDistance * 1.5), 400); // Maximum distance is 150% of last detected

                Console.WriteLine("Updated Distances - Min: {0}, Max: {1}", _minDistanceFromPerson, _maxDistanceFromPerson);
                IsFollowingPerson = true;
            }
            // else
            // {
            //     Stop();
            //     HasPerformedScan = true;
            // }
        }
    }

    void FollowPerson()
    {
        int distance = _obstacleDetectionSystem.Distance;

        // Check if the person is within the updated range
        if (distance > _minDistanceFromPerson && distance < _maxDistanceFromPerson)
        {
            EnqueDrive(Direction.Forward, 50); // Keep following the person
            _loggingSystem.LogToLcd("Following person...");

            // Dynamically adjust the follow range as the person moves
            _minDistanceFromPerson = Math.Max((int)(distance * 0.8), 20); // Minimum distance is 80% of current
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

            if (IsFollowingPerson)
            {
                FollowPerson();
            }

            if (_isScanning)
            {
                PerformScanning();
            }

            base.Update();
        }
    }

}




