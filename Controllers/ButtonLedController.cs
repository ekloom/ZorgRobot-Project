
using Avans.StatisticalRobot;

namespace RobotProject.Controllers;

public class ButtonLedController : IUpdatable
{
  bool wasButtonPressed;
  bool isButtonPressed;

  Button button;
  Led led;
  ButtonStatus buttonStatus;

  DateTime? buttonPressStartTime;

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
    isButtonPressed = button.GetState() == "Pressed";


    if (isButtonPressed)
    {

      if (!wasButtonPressed)
      {
        wasButtonPressed = true;
        buttonStatus.IsSwitchedOn = !buttonStatus.IsSwitchedOn;
      }

      if (buttonPressStartTime == null)
      {
        buttonPressStartTime = DateTime.Now;
      }
      else
      {
        buttonStatus.TimePressed = (int)(DateTime.Now - buttonPressStartTime.Value).TotalMilliseconds;
        Console.WriteLine($"Button was pressed for {buttonStatus.TimePressed} milliseconds.");
      }
    }
    else if (buttonPressStartTime != null)
    {
      buttonStatus.TimePressed = (int)(DateTime.Now - buttonPressStartTime.Value).TotalMilliseconds;
      buttonPressStartTime = null; // Reset the timer
      Console.WriteLine($"Button was pressed for {buttonStatus.TimePressed} milliseconds.");
    }

    if (buttonStatus.IsSwitchedOn)
    {
      led.SetOn();
    }
    else
    {
      led.SetOff();
    }

    if (!isButtonPressed && wasButtonPressed)
    {
      wasButtonPressed = false;
      Console.WriteLine("Set wasButtonPressed variable to false");
    }

    Robot.Wait(50);
  }
}


public class ButtonStatus
{
  public int TimePressed { get; internal set; }

  public bool IsSwitchedOn { get; internal set; }

}