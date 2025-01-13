using RobotProject.Controllers;

namespace RobotProject.ControlSystems;

public class InteractionSystem : IUpdatable
{

  ButtonStatus ButtonStatus;

  ButtonLedController buttonLedController;

  private DateTime queryStartTime;
  private TimeSpan timeout;

  private string QueryID;
  private bool IsRequesting = false;
  public string Response { get; private set; }

  public InteractionSystem(ButtonLedController buttonLedController, int queryTimeout)
  {
    this.buttonLedController = buttonLedController;
    timeout = TimeSpan.FromSeconds(queryTimeout); // 10 seconds timeout
  }

  public void Query(string query)
  {

    if (!IsRequesting)
    {
      Response = ""; // Reset response
      QueryID = query; // Store query ID
      IsRequesting = true;
      queryStartTime = DateTime.Now;
      PlayWavForQuery(query);
    }

    ButtonStatus = buttonLedController.GetButtonStatus(); // Get button status

  }

  public void Update()
  {
    if (DateTime.Now - queryStartTime > timeout)
    {
      Response = "No response";
      IsRequesting = false;
    }

    if (IsRequesting)
    {
      if (ButtonStatus.TimesPressed >= 2)
      {
        IsRequesting = false;
        Response = $"no; Query:{QueryID}";
      }

      if (ButtonStatus.TimesPressed == 1)
      {
        IsRequesting = false;
        Response = $"yes; Query:{QueryID}";
      }

    }
  }

  private void PlayWavForQuery(string query)
  {
    throw new NotImplementedException();
  }

}
