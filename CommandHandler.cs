namespace RobotProject;

public class CommandHandler
{

  Dictionary<string, Action> _commands;

  public CommandHandler()
  {
    _commands = new Dictionary<string, Action>();
  }


  /// <summary>
  /// This will add a new command to the list of commands
  /// </summary>
  /// <param name="commandName"></param>
  /// <param name="action"></param>
  public void AddCommand(string commandName, Action action)
  {
    _commands.Add(commandName, action);
  }

  /// <summary>
  /// Executes the action associated with the specified command.
  /// If the command doesn't exist, it logs an error or provides feedback.
  /// </summary>
  /// <param name="commandName"></param>
  public void ExecuteCommand(string commandName)
  {
    if (_commands.ContainsKey(commandName))
    {
      _commands[commandName]();
    }
    else
    {
      Console.WriteLine($"Command '{commandName}' not found.");
    }
  }

}
