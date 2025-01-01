using Avans.StatisticalRobot;
using RobotProject.ControlSystems;
using RobotProject.Controllers;


namespace RobotProject
{
    public class RobotManager : IUpdatable
    {
        // private readonly List<IUpdatable> _components;

        // Controllers and systems

        private readonly DrivingSystem drivingSystem;


        protected readonly ButtonLedController buttonLedSystem;

        // Actuators
        private readonly LCD16x2 lCD16X2;

        private readonly string robotName;


        private bool IsAutoDriving;


        public RobotManager()
        {
            // Initialize components

            buttonLedSystem = new ButtonLedController(6);
            lCD16X2 = new LCD16x2(0x3E);

            drivingSystem = new DrivingSystem(lCD16X2);

            robotName = "Memento";
        }

        public void Init()
        {

            // Display welcome message
            lCD16X2.SetText($"Welkom! Ik ben {robotName}!");
        }



        public void Update()
        {
            // Perform component updates
            drivingSystem.Update();
            buttonLedSystem.Update();

            if (buttonLedSystem.IsSwitchedOn())
            {
                IsAutoDriving = true;
            }
            else
            {
                IsAutoDriving = false;
            }

            // Example behavior: Follow a target
            if (IsAutoDriving)
            {
                drivingSystem.FollowTarget();
            }
            else
            {
                // Manual Control via MQTT

                // Needs continuation of input for the direction otherwise stop the motor

                // Command: Stop
                drivingSystem.Stop();

                // Command: Forward
                // drivingSystem.Drive(Direction.Forward, 100);
                // Command: Turn forward left diagonal
                // drivingSystem.Drive(Direction.Forward | Direction.Left, 100);
                // Command: Turn forward right diagonal
                // drivingSystem.Drive(Direction.Forward | Direction.Right, 100);

                // Command: Backwards
                // drivingSystem.Drive(Direction.Backwards, 100);
                // Command: Turn Backwards left diagonal
                // drivingSystem.Drive(Direction.Backwards | Direction.Left, 100);
                // Command: Turn Backwards right diagonal
                // drivingSystem.Drive(Direction.Backwards | Direction.Right, 100);


                drivingSystem.Update();
            }


        }


    }
}
