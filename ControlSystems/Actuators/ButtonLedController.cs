
using Avans.StatisticalRobot;

namespace RobotProject.ControlSystems.Actuators;

public class ButtonLedController : IUpdatable
{
  bool isSchakelaarAan = false;
  bool wasButtonPressed = false;

  Button button;

  public ButtonLedController(int ButtonPinNumber)
  {
    button = new Button(ButtonPinNumber);
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
    bool isButtonPressed = button.GetState() == "Pressed";

    if (isButtonPressed && !wasButtonPressed)
    {
      wasButtonPressed = true;
      isSchakelaarAan = !isSchakelaarAan;
    }

    if (!isButtonPressed && wasButtonPressed)
    {
      wasButtonPressed = false;
      Console.WriteLine("Set wasButtonPressed variable to false");
    }

    Robot.Wait(50);
  }
}