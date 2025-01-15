using Avans.StatisticalRobot;
using RobotProject.ControlSystems.DrivingStrategySystem;
using RobotProject.ControlSystems.DrivingStrategySystem.AutonomeStates;
using RobotProject.ControlSystems.DrivingStrategySystem.FollowPersonStates;
using RobotProject.ControlSystems.Util;

namespace RobotProject.ControlSystems;

public class DrivingSystem : MotorController
{

    private DrivingContext _context;
    private IDrivingState _currentState;

    public DrivingMode DrivingMode { get; set; }

    private bool isEmergencyStop;

    private readonly PIRMotion PIRMotion;

    public DrivingSystem(ObstacleDetectionSystem obstacleDetectionSystem, LoggingSystem loggingSystem, PIRMotion pIRMotion)
    {

        PIRMotion = pIRMotion;
        _context = new DrivingContext(obstacleDetectionSystem, loggingSystem, pIRMotion); // Pass the system to the context
        _currentState = null; // Initial state

        DrivingMode = DrivingMode.Idle;
        SetMotorSpeed(0, 0); // Set motor speed to 0
        ResetMotors();
    }

    public void Reset()
    {
        ResetMotors();
        isEmergencyStop = false; // Reset the emergency stop flag
        DrivingMode = DrivingMode.Idle;
        _currentState = new AutonomeIdleState();
    }

    public void EmergencyStop()
    {
        isEmergencyStop = true; // set the emergency stop flag to true
        SetMotorSpeed(0, 0); // stops the motors
        ResetMotors();
        System.Console.WriteLine("EmergencyStop activated!");
    }

    public void SetState(IDrivingState newState)
    {
        _currentState = newState;
    }

    private void InitializeStateForMode()
    {
        switch (DrivingMode)
        {
            case DrivingMode.Autonome:
                // Only set to AutonomeIdleState if the current state is not AutonomeMovingForwardState
                // and the system hasn't just transitioned from FollowPerson mode
                if (_currentState == null || (_currentState is FollowPersonIdleState))
                {
                    _currentState = new AutonomeIdleState();
                }
                break;

            case DrivingMode.FollowPerson:
                if (_currentState == null || (_currentState is AutonomeIdleState))
                {
                    _currentState = new FollowPersonIdleState();
                }
                break;

            case DrivingMode.Idle:
                Stop();
                break;
        }
    }


    public override void Update()
    {
        if (!isEmergencyStop)
        {

            InitializeStateForMode();
            if (_currentState != null)
            {
                _currentState.Handle(this, _context);
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

