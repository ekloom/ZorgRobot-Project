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

    private int _maxDistanceFromPerson = 150;
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

    public void StarScanning()
    {
        _totalRotation = 0;
        _scanstep = 30;
        _isScanning = true;
        HasPerformedScan = false;
        _lastDetectedDistance = float.MaxValue;
        Stop();
    }

    void PerformScanning()
    {
        if (!_isScanning) return;

        Drive(Direction.Left, (short)_scanstep);
        _totalRotation += _scanstep;

        int currentDistance = _obstacleDetectionSystem.Distance;

        if (currentDistance < _lastDetectedDistance && currentDistance < _maxDistanceFromPerson)
        {
            _lastDetectedDistance = currentDistance;
            _targetAngle = _totalRotation;
        }
        System.Console.WriteLine("TotalDistance: {0}", _totalRotation);
        System.Console.WriteLine("targetAngle: {0}", _targetAngle);
        System.Console.WriteLine("lastDetectedDistance: {0}", _lastDetectedDistance);

        if (_totalRotation >= 360 || _lastDetectedDistance < _maxDistanceFromPerson - (_maxDistanceFromPerson / 2))
        {

            _isScanning = false;
            if (_lastDetectedDistance < 200)
            {
                RotateToTargetAngle();
                Drive(Direction.Forward, 50);
            }
            else
            {
                Stop();
                HasPerformedScan = true;
            }
        }

    }

    void RotateToTargetAngle()
    {
        int adjusments = _targetAngle - _totalRotation;

        Direction dir = adjusments > 0 ? Direction.Right : Direction.Left;
        Drive(dir, (short)adjusments);
    }

    void FollowPerson()
    {
        int distance = _obstacleDetectionSystem.Distance;

        if (distance > _minDistanceFromPerson && distance < _maxDistanceFromPerson)
        {

            Drive(Direction.Forward, 100);
            _loggingSystem.LogToLcd("Following target...");
        }
        else
        {
            // Scan area
            _loggingSystem.LogToLcd("Lost person start scanning");
            StarScanning();
        }
    }


    public override void Update()
    {
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




