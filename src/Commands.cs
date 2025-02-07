
using VCS.Core;

public class StageCommand : ICommandExecutionPolicy
{
    public string CallName { get => "stage"; }


    public  void Execute(object[]? args)
    {   
        Console.WriteLine("CALLED THIS SHIT");
        if (args is null)
        {
            Log.WriteLine("Wrong argument pattern, feed files / directories to stage.");
            return;
        }
        // user starts app from command line as -- >  startapp arg1 arg2 arg3 arg4 arg5 arg6 arg7 argn.....
        // we call the function with reflections like this:   func(name, args)

        // function percieved args as (arg1, arg2, arg3, arg, ....)  instead of ([arg1,arg2,arg3,arg4,arg5...])
        // --> catastrophic failure if unhandled, because this is a parameter count mismatch, 
        // the function expected a list of strings, but was passed with strings as seprate objects

        string[] arr = new string[args.Length];

        #pragma warning disable  CS8601
        for (int i = 0; i < args.Length; i++)
            arr[i] = args[i] as string;
        #pragma warning restore
        Controller.Stage(arr);
    }

}