#pragma warning disable CA1050
public interface ICommandExecutionPolicy {
#pragma warning restore
    
    public string CallName {get ;  } 
    public  abstract  void Execute(object[] args);
}

