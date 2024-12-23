using Avans.StatisticalRobot;
using RobotProject.Util;

namespace RobotProject.ControlSystems
{
    internal class MotorController : IUpdatable
    {

        private bool isMovingForward;

        private short CurrentMotorSpeedL;
        private short CurrentMotorSpeedR;

        public MotorController()
        {
            isMovingForward = false;
            CurrentMotorSpeedL = 0;
            CurrentMotorSpeedR = 0;
        }

        public void MoveForward()
        {
            isMovingForward = true;
            CurrentMotorSpeedL = 100;
            CurrentMotorSpeedR = 100;
            Robot.Motors(CurrentMotorSpeedL, CurrentMotorSpeedR);
        }

        public void Stop()
        {
            isMovingForward = false;

            int steps = 100; // Number of steps in the curve
            short targetSpeed = 0;

            if (CurrentMotorSpeedL > 10 && CurrentMotorSpeedR > 10)
            {
                for (int i = 0; i <= steps; i++)
                {
                    float t = (float)i / (float)steps; // Normalized time [0, 1]
                                                       // Calculate easing value (quadratic decay)
                    CurrentMotorSpeedL = (short)MathFunctions.Interpolate(CurrentMotorSpeedL, targetSpeed, t);
                    CurrentMotorSpeedR = (short)MathFunctions.Interpolate(CurrentMotorSpeedR, targetSpeed, t);
                    // Wait before updating the speed again
                    Robot.Motors(CurrentMotorSpeedL, CurrentMotorSpeedR);
                    Console.WriteLine("Motor : {0}, Motor : {1}", CurrentMotorSpeedL, CurrentMotorSpeedR);
                    if (CurrentMotorSpeedL == targetSpeed && CurrentMotorSpeedR == targetSpeed) i = steps;
                    Robot.Wait(50);
                }

            }

            Robot.Motors(targetSpeed, targetSpeed);

        }

        public void EmergencyStop()
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
