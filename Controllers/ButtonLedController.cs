
using System.Runtime.InteropServices;
using Avans.StatisticalRobot;

namespace RobotProject.Controllers;

public class ButtonLedController : IUpdatable
{

  Button button;
  Led led;
  ButtonStatus buttonStatus;

  DateTime? buttonPressStartTime;
  DateTime? lastPressed;

  bool wasButtonPressed;

  const int debounceTimeMs = 250; // The debounce time in milliseconds
  const int longpressThreshold = 1000; // Threshold for the long press in milliseconds
  const int resetTimeOutMs = 3000; // The time out in milliseconds 


  public ButtonLedController(int ButtonPinNumber)
  {
    button = new Button(ButtonPinNumber);
    led = new Led(ButtonPinNumber - 1);
    wasButtonPressed = false;
    buttonStatus = new ButtonStatus();
    buttonPressStartTime = null;
  }

  public ButtonStatus GetButtonStatus() => buttonStatus;

  public void Update()
  {
    buttonStatus.isButtonPressed = button.GetState() == "Pressed";


    if (buttonStatus.isButtonPressed)
    {

      if (buttonPressStartTime == null)
      {
        buttonPressStartTime = DateTime.Now;
      }


      if (lastPressed == null || (DateTime.Now - lastPressed.Value).TotalMilliseconds >= debounceTimeMs)
      {
        if ((DateTime.Now - buttonPressStartTime.Value).TotalMilliseconds <= longpressThreshold)
        {
          buttonStatus.TimesPressed++;
          System.Console.WriteLine("Button was pressed {0} times.", buttonStatus.TimesPressed);
        }

        lastPressed = DateTime.Now;
      }

      if (!wasButtonPressed)
      {
        wasButtonPressed = true;
        buttonStatus.IsSwitchedOn = !buttonStatus.IsSwitchedOn;
      }

      if ((DateTime.Now - buttonPressStartTime.Value).TotalMilliseconds >= longpressThreshold)
      {
        buttonStatus.PressingDuration = (int)(DateTime.Now - buttonPressStartTime.Value).TotalMilliseconds;
        Console.WriteLine($"Button was pressed for {buttonStatus.PressingDuration} milliseconds.");
      }

    }
    else
    {
      if (buttonPressStartTime != null)
      {
        buttonPressStartTime = null; // Reset the timer
        buttonStatus.PressingDuration = 0;
        Console.WriteLine($"Button was pressed for {buttonStatus.PressingDuration} milliseconds.");
      }

      wasButtonPressed = false;

      if (lastPressed != null && (DateTime.Now - lastPressed.Value).TotalMilliseconds > resetTimeOutMs)
      {
        buttonStatus.TimesPressed = 0;
        lastPressed = null;
        System.Console.WriteLine("TimesPressed was reset to 0 due to inactivity.");
      }
    }

    // if (buttonStatus.IsSwitchedOn)
    // {
    //   led.SetOn();
    // }
    // else
    // {
    //   led.SetOff();
    // }

    // Standard debounce time
    // Robot.Wait(50);
  }
}


public class ButtonStatus
{
  public int TimesPressed { get; internal set; }

  public int PressingDuration { get; internal set; }

  public bool IsSwitchedOn { get; internal set; }

  public bool isButtonPressed { get; internal set; }
}