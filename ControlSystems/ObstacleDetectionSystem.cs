using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems
{
    public class ObstacleDetectionSystem : IUpdatable
    {

        public int Distance { get; private set; }
        private int scanInterval = 250;

        private Ultrasonic distanceSensor;
        private PeriodTimer scanIntervalTimer;

        public ObstacleDetectionSystem(Ultrasonic _Ultrasonic)
        {
            distanceSensor = _Ultrasonic;
            scanIntervalTimer = new PeriodTimer(scanInterval);
        }

        public void Update()
        {
            if (scanIntervalTimer.Check())
            {
                // Sets the distance field to the current detected distance from the robot
                Distance = distanceSensor.GetUltrasoneDistance();
            }
        }
    }
}
