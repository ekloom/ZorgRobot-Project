using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems
{
    public class ObstacleDetectionSystem : IUpdatable
    {

        public int Distance { get; private set; }
        private int scanInterval = 500;


        private Ultrasonic distanceSensor;
        private PeriodTimer scanIntervalTimer;

        public ObstacleDetectionSystem(int UltrasonicPinNumber)
        {
            distanceSensor = new Ultrasonic(UltrasonicPinNumber);
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
