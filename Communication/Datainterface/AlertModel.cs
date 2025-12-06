using System;

namespace RobotProject.Communication.Datainterface;

public class AlertModel
{
  public Guid Id { get; set; }
  public string Title { get; set; }
  public DateTime Date { get; set; }
}
