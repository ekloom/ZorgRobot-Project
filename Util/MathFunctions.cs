using System;

namespace RobotProject.Util;

public static class MathFunctions
{

  static float EaseOutCubic(float t)
  {
    return 1.0f - (float)Math.Pow(1.0f - t, 3);
  }

  // Function to interpolate from start to end using the easing function
  public static float Interpolate(float start, float end, float t)
  {
    float easedT = EaseOutCubic(t);
    return start + (end - start) * easedT;
  }

}
