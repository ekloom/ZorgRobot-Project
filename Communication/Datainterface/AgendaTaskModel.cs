using System;

namespace RobotProject.Services.Datainterface;

public class AgendaTaskModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Interval { get; set; }
}
