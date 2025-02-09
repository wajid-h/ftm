using System.Reflection;
using System.Security.Cryptography;

namespace VCS
{
    public class FTMCoreLauncher
    {
        
        private static Dictionary<string, MethodInfo> commands = []; 

        public static void Main(params string[] args){


            commands =  new CommandLoader().LoadCommands();
            
            foreach(var command in commands){
                Log.WriteLine(command.ToString());
            }
            
            //Execute(args); 

        }

        public static bool Execute(string[] args){  

            if(args is null || args.Length == 0  ) return false;


            List<string> launchArgs  =  [..args] ;
            string commandCall =  launchArgs[0] ;            
            
            Log.WriteLine($"Running {commandCall}");
            
            launchArgs.RemoveAt(0) ;
            commands.First(query=>query.Key ==  commandCall).Value.Invoke(null, [..launchArgs]);;    

            return true ;
        }


    }
}