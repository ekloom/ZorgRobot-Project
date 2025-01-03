using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Util;

public class MotorFuntions
{
  public static void EaseOutMotors(ref short currentMotorSpeedL, ref short currentMotorSpeedR, short targetSpeedLeft, short targetSpeedRight)
  {
    const float stepTimeMs = 50.0f; // Time per step in milliseconds
    const float durationMs = 3000.0f; // Total duration for ease out (adjust as needed)
    float elapsedTime = 0.0f;

    while (elapsedTime <= durationMs)
    {
      // Calculate the progress normalized to [0, 1]
      float t = elapsedTime / durationMs;

      // Apply the easing function
      float easedT = MathFunctions.EaseOutCubic(t);

      // Interpolate motor speeds
      currentMotorSpeedL = (short)(currentMotorSpeedL + (targetSpeedLeft - currentMotorSpeedL) * easedT);
      currentMotorSpeedR = (short)(currentMotorSpeedR + (targetSpeedRight - currentMotorSpeedR) * easedT);

      // Update motor speed
      SetMotorSpeed(currentMotorSpeedL, currentMotorSpeedR);
      Console.WriteLine("Motor L: {0}, Motor R: {1}", currentMotorSpeedL, currentMotorSpeedR);

      // Break the loop if speeds are close enough to target
      if (Math.Abs(currentMotorSpeedL - targetSpeedLeft) <= 1 && Math.Abs(currentMotorSpeedR - targetSpeedRight) <= 1)
      {
        break;
      }

      // Wait and increment time
      Robot.Wait((int)stepTimeMs / 2);
      elapsedTime += stepTimeMs;
    }

    // Ensure the final speeds are set to target
    currentMotorSpeedL = targetSpeedLeft;
    currentMotorSpeedR = targetSpeedRight;
    SetMotorSpeed(targetSpeedLeft, targetSpeedRight);
  }

  public static void SetMotorSpeed(short SpeedL, short SpeedR)
  {
    // Apply speed ti the motors
    Robot.Motors(SpeedL, SpeedR);
  }
}
