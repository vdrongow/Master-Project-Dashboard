using System;

public class CommandBase
{
    public string CommandId { get; private set; }
    public string CommandDescription { get; private set; }
    public string CommandFormat { get; private set; }
    
    public CommandBase(string commandId, string commandDescription, string commandFormat)
    {
        CommandId = commandId;
        CommandDescription = commandDescription;
        CommandFormat = commandFormat;
    }
}

public class Command : CommandBase
{
    private Action _command;
    
    public Command(string commandId, string commandDescription, string commandFormat, Action command) : base(commandId, commandDescription, commandFormat)
    {
        _command = command;
    }
    
    public void InvokeCommand()
    {
        _command.Invoke();
    }
}

public class Command<T> : CommandBase
{
    private Action<T> _command;
    
    public Command(string commandId, string commandDescription, string commandFormat, Action<T> command) : base(commandId, commandDescription, commandFormat)
    {
        _command = command;
    }
    
    public void InvokeCommand(T value)
    {
        _command.Invoke(value);
    }
}