using Avans.StatisticalRobot;
using RobotProject.Util;

namespace RobotProject.ControlSystems
{
    internal class MotorController : IUpdatable
    {

        private bool isMotorActive;
        private bool isEmergencyStop;

        private short CurrentMotorSpeedL;
        private short CurrentMotorSpeedR;

        short targetSpeedR = 0;
        short targetSpeedL = 0;

        public MotorController()
        {
            isMotorActive = false;
            isEmergencyStop = false;
            CurrentMotorSpeedL = 0;
            CurrentMotorSpeedR = 0;
        }

        public void MoveForward()
        {
            isMotorActive = true;
            targetSpeedR = 100;
            targetSpeedL = 100;
            Robot.Motors(CurrentMotorSpeedL, CurrentMotorSpeedR);
        }

        public void Stop()
        {
            targetSpeedR = 0;
            targetSpeedL = 0;
        }

        private void SetMotorSpeed(short SpeedL = -1, short SpeedR = -1)
        {
            Robot.Motors(SpeedL == -1 ? CurrentMotorSpeedL : SpeedL, SpeedR == -1 ? CurrentMotorSpeedR : SpeedR);
        }

        private void EaseOutMotors(short targetSpeedLeft, short targetSpeedRight)
        {
            int steps = 100; // Number of steps in the curve

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / (float)steps; // Normalized time [0, 1]

                // Interpolate the left motor speed
                CurrentMotorSpeedL = (short)MathFunctions.InterpolateWithEaseOutCubic(CurrentMotorSpeedL, targetSpeedLeft, t);

                // Interpolate the right motor speed
                CurrentMotorSpeedR = (short)MathFunctions.InterpolateWithEaseOutCubic(CurrentMotorSpeedR, targetSpeedRight, t);

                // Updates motor speed
                SetMotorSpeed();
                Console.WriteLine("Motor : {0}, Motor : {1}", CurrentMotorSpeedL, CurrentMotorSpeedR);

                // Sets the iterator equal to steps if the targetSpeed is already met
                if (CurrentMotorSpeedL == targetSpeedL && CurrentMotorSpeedR == targetSpeedR) i = steps;

                // Wait before updating again
                Robot.Wait(50);
            }

            // Ensures that the speed of the motor is set to the target speed
            SetMotorSpeed(targetSpeedL, targetSpeedR);
            isMotorActive = false;
        }


        public void EmergencyStop()
        {
            isEmergencyStop = true;
            Robot.Motors(0, 0);
        }


        public void Update()
        {
            // Handle periodic updates if necessary
            if (isMotorActive && !isEmergencyStop)
            {
                if (CurrentMotorSpeedL < targetSpeedL && CurrentMotorSpeedL < targetSpeedR)
                {
                    EaseOutMotors(targetSpeedL, targetSpeedR);
                }

                if (CurrentMotorSpeedL > targetSpeedL && CurrentMotorSpeedR > targetSpeedL)
                {
                    EaseOutMotors(targetSpeedL, targetSpeedR);
                }
            }
        }
    }
}
