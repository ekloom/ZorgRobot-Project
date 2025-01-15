

using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.DrivingStrategySystem
{
    public class DrivingContext
    {
        public ObstacleDetectionSystem ObstacleDetectionSystem { get; }
        public LoggingSystem LoggingSystem { get; }
        public PIRMotion PIRMotion { get; }
        public double PreviousDistance { get; set; }
        public int SafeDistanceThreshold { get; set; } = 40;

        // Constructor with DI for ObstacleDetectionSystem
        public DrivingContext(ObstacleDetectionSystem obstacleDetectionSystem, LoggingSystem loggingSystem, PIRMotion pIRMotion)
        {
            ObstacleDetectionSystem = obstacleDetectionSystem;
            LoggingSystem = loggingSystem;
            PIRMotion = pIRMotion;
        }

        internal bool HasTurnedEnough()
        {
            // Check the current distance from the ultrasonic sensor
            double frontDistance = ObstacleDetectionSystem.Distance;

            // Return true if the front distance is above a safe threshold
            return frontDistance > SafeDistanceThreshold;
        }

        internal bool IsDistanceChanging(double currentDistance, double previousDistance)
        {
            const double tolerance = 0.1;
            return Math.Abs(currentDistance - previousDistance) > tolerance;
        }

    }

}
