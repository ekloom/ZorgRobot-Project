
using Avans.StatisticalRobot;

namespace RobotProject.Controllers;

public class ButtonLedController : IUpdatable
{
  bool isSchakelaarAan;
  bool wasButtonPressed;
  bool isButtonPressed;

  Button button;
  Led led;

  public ButtonLedController(int ButtonPinNumber)
  {
    button = new Button(ButtonPinNumber);
    led = new Led(ButtonPinNumber - 1);
    isSchakelaarAan = false;
    wasButtonPressed = false;
  }

  public bool HasBeenPressed()
  {
    return wasButtonPressed;
  }

  public bool IsSwitchedOn()
  {
    return isSchakelaarAan;
  }

  public void Update()
  {
    isButtonPressed = button.GetState() == "Pressed";

    if (isButtonPressed && !wasButtonPressed)
    {
      wasButtonPressed = true;
      isSchakelaarAan = !isSchakelaarAan;
    }

    if (isSchakelaarAan)
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