namespace RobotProject.Communication.Datainterface
{
    public class ActivityModel : ISqlDataObject
    {
        public int Id { get; private set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
