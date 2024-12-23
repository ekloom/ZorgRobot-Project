using Avans.StatisticalRobot;
using RobotProject.ControlSystems;
using RobotProject.ControlSystems.Actuators;


namespace RobotProject
{
    public class RobotManager : IUpdatable
    {
        private readonly List<IUpdatable> _components;

        // Controllers and systems
        private readonly MotorController motorController;
        private readonly ObstacleDetectionSystem obstacleDetectionSystem;
        protected readonly ButtonLedController buttonLedController;

        // Actuators
        private readonly LCD16x2 lCD16X2;

        private readonly string robotName;


        private bool IsFollowingTarget;


        public RobotManager()
        {
            // Initialize components
            motorController = new MotorController();
            obstacleDetectionSystem = new ObstacleDetectionSystem(16);
            buttonLedController = new ButtonLedController(6);


            lCD16X2 = new LCD16x2(0x3E);

            robotName = "Memento";

            // Add all updatable components
            _components = new List<IUpdatable>
            {
                buttonLedController,
                obstacleDetectionSystem,
                motorController,
            };

            // Display welcome message
            lCD16X2.SetText($"Welkom! Ik ben {robotName}!");
        }

        public void Update()
        {
            // Perform component updates
            foreach (var component in _components)
            {
                component.Update();
            }

            if (buttonLedController.IsSwitchedOn())
            {
                IsFollowingTarget = true;
            }
            else
            {
                IsFollowingTarget = false;
            }

            // Example behavior: Follow a target
            if (IsFollowingTarget)
            {
                FollowTarget();
            }
            else
            {
                motorController.Stop();
            }

        }

        private void FollowTarget()
        {
            if (!obstacleDetectionSystem.IsPathClear())
            {
                motorController.Stop();
                Console.WriteLine("Obstacle detected!");
                lCD16X2.SetText("Obstacle detected!");
            }
            else
            {
                motorController.MoveForward();
                Console.WriteLine("Following target...");
                lCD16X2.SetText("Following target...");
            }
        }
    }
}
