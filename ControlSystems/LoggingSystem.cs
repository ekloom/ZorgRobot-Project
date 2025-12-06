using System;
using System.Collections.Generic;
using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems;

public class LoggingSystem : IUpdatable
{
  private LCD16x2 LCD;

  private string? tempMessage;
  private string _currentMessage = "";
  private int _currentIndex = 0;
  private bool _isSliding = false;

  private const int MaxVisibleCharacters = 32; // Max aantal tekens zichtbaar op het LCD
  private DateTime _lastSlideTime;
  private const int SlideInterval = 500; // Sliding snelheid in milliseconden

  public LoggingSystem(LCD16x2 led)
  {
    LCD = led;
    tempMessage = "";
    _lastSlideTime = DateTime.Now;
  }

  public void LogToLcd(string message, bool withSlideShow = false)
  {
    if (message == tempMessage) return; // Voorkom duplicatie van hetzelfde bericht
    tempMessage = message;

    // Controleer automatisch of de sliding-modus nodig is
    if (message.Length > MaxVisibleCharacters || withSlideShow)
    {
      _currentMessage = message + new string(' ', MaxVisibleCharacters); // Voeg spaties toe voor continue sliding
      _currentIndex = 0;
      _isSliding = true;
    }
    else
    {
      // Log direct zonder slideshow
      LCD.SetText(message);
      _isSliding = false;
    }

    System.Console.WriteLine($"Message Logged on LCD: {message}");
  }

  public void Update()
  {
    if (!_isSliding || string.IsNullOrEmpty(_currentMessage)) return;

    // Controleer of er genoeg tijd is verstreken om de volgende slide te tonen
    if ((DateTime.Now - _lastSlideTime).TotalMilliseconds >= SlideInterval)
    {
      // Bereken het zichtbare deel van de boodschap
      int startIndex = _currentIndex % _currentMessage.Length; // Circulaire index
      string visiblePart;

      if (startIndex + MaxVisibleCharacters <= _currentMessage.Length)
      {
        // Geen overlap
        visiblePart = _currentMessage.Substring(startIndex, MaxVisibleCharacters);
      }
      else
      {
        // Overlap: voeg begin van het bericht toe
        int part1Length = _currentMessage.Length - startIndex;
        visiblePart = _currentMessage.Substring(startIndex, part1Length) +
                      _currentMessage.Substring(0, MaxVisibleCharacters - part1Length);
      }

      LCD.SetText(visiblePart);

      // Beweeg de index vooruit
      _currentIndex++;

      _lastSlideTime = DateTime.Now; // Update de tijd
    }
  }
}
