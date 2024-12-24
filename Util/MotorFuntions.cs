using Avans.StatisticalRobot;

namespace RobotProject.Util;

public class MotorFuntions
{
  public static void EaseOutMotors(ref short CurrentMotorSpeedL, ref short CurrentMotorSpeedR, short targetSpeedLeft, short targetSpeedRight)
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
      SetMotorSpeed(CurrentMotorSpeedL, CurrentMotorSpeedR);
      Console.WriteLine("Motor : {0}, Motor : {1}", CurrentMotorSpeedL, CurrentMotorSpeedR);

      // Sets the iterator equal to steps if the targetSpeed is already met
      if (CurrentMotorSpeedL == targetSpeedLeft && CurrentMotorSpeedR == targetSpeedRight) i = steps;

      // Wait before updating again
      Robot.Wait(50);
    }

    // Ensures that the speed of the motor is set to the target speed
    CurrentMotorSpeedL = targetSpeedLeft;
    CurrentMotorSpeedR = targetSpeedRight;
    SetMotorSpeed(targetSpeedLeft, targetSpeedRight);
  }

  public static void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    Robot.Motors(SpeedL, SpeedR);
  }
}
