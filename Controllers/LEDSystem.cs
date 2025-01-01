using Avans.StatisticalRobot;

namespace RobotProject.Controllers
{
    public class LEDController
    {


        public static void BlinkLed(LedColors ledColors)
        {
            byte[] color = GetColor(ledColors);

            // Pass array elements individually
            if (color.Length == 3) // Ensure the array has 3 elements
            {
                Robot.LEDs(color[0], color[1], color[2]);
            }
            else
            {
                throw new ArgumentException("GetColor must return exactly 3 values for RGB.");
            }

            Robot.Wait(100);
            Robot.LEDs(0, 0, 0);
        }

        static byte[] GetColor(LedColors ledColors)
        {
            switch (ledColors)
            {
                case LedColors.Red:
                    return new byte[] { 255, 0, 0 }; // RGB values for red
                case LedColors.Green:
                    return new byte[] { 0, 255, 0 }; // RGB values for green
                case LedColors.Blue:
                    return new byte[] { 0, 0, 255 }; // RGB values for blue
                default:
                    throw new ArgumentException("Invalid LedColor.");
            }
        }


    }


    public enum LedColors
    {
        Red = 0,
        Green,
        Blue,

    }

}
