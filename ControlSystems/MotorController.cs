using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems
{
    internal class MotorController : IUpdatable
    {

        private bool isMovingForward;

        public MotorController()
        {
            isMovingForward = false;
        }

        public void MoveForward()
        {
            isMovingForward = true;
            Robot.Motors(100, 100);
        }

        public void Stop()
        {
            isMovingForward = false;
            Robot.Motors(0, 0);
        }


        public void Update()
        {
            // Handle periodic updates if necessary
            if (isMovingForward)
            {
                // Placeholder for motor diagnostics or telemetry
                Console.WriteLine("Motor is running...");
            }
        }
    }
}
