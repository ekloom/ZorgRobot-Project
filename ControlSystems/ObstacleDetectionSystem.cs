using Avans.StatisticalRobot;
using RobotProject.ControlSystems.Actuators;

namespace RobotProject.ControlSystems
{
    internal class ObstacleDetectionSystem : IUpdatable
    {

        private int _distance;
        private int scanInterval = 500;

        private int detectionThreshold;

        private Ultrasonic distanceSensor;
        private PeriodTimer scanIntervalTimer;

        public ObstacleDetectionSystem(int UltrasonicPinNumber)
        {
            distanceSensor = new Ultrasonic(UltrasonicPinNumber);
            scanIntervalTimer = new PeriodTimer(scanInterval);

            detectionThreshold = 20;
        }

        public bool IsPathClear()
        {
            if (GetDistanceToSensor() < detectionThreshold)
            {
                return true;
            }

            return false;
        }

        public int GetDistanceToSensor()
        {
            return _distance;
        }

        public void Update()
        {
            if (scanIntervalTimer.Check())
            {
                // Sets the distance field to the current detected distance from the robot
                _distance = distanceSensor.GetUltrasoneDistance();
                // Give a sign of detecting something
            }
        }
    }
}
