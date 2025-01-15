using System;
using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems;

public class LoggingSystem
{


  private LCD16x2 LCD;

  static string? tempMessage;

  public LoggingSystem(byte pinNumber)
  {
    LCD = new LCD16x2(pinNumber);
    tempMessage = "";
  }

  public void LogToLcd(string message)
  {
    if (message == tempMessage) return; // prevent infinite loop
    tempMessage = message; // store message to prevent infinite loop

    LCD.SetText(message);

    System.Console.WriteLine($"Message Logged on LCD: {message}");
  }

}
