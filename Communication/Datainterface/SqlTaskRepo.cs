using System;

namespace RobotProject.Services.Datainterface;

public class SqlTaskRepo
{
    private readonly SqlInterface sqlInterface;

    public List<AgendaTaskModel> ListOfTask { get; private set; }

    public SqlTaskRepo()
    {
        sqlInterface = new SqlInterface();
    }

    public List<AgendaTaskModel> UpdateList()
    {
        ListOfTask = sqlInterface.GetListOfData<AgendaTaskModel>("AgendaTask", "[Title]");
        // Sort the list by StartTime in ascending order
        ListOfTask = ListOfTask.OrderBy(task => task.StartTime).ToList();
        return ListOfTask;
    }


    public void SaveTask(AgendaTaskModel task)
    {
        sqlInterface.SaveData("AgendaTask", task);
    }
}
