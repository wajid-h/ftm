using System.Reflection;
namespace FTM 
{

    public class CommandRegistery{

        private  Dictionary<string, MethodInfo> commands =  [];
        public  Dictionary<string, MethodInfo>  Commands {get =>  commands ??=LoadRegistery();  }

        

        private Dictionary<string, MethodInfo> LoadRegistery(){

            Dictionary<string, MethodInfo> commands_register=  [] ; 

            Assembly  executingAssembly =  Assembly.GetExecutingAssembly();

            Type[]  activeTypes = executingAssembly.GetTypes();


            foreach (Type type  in activeTypes){

                var  methdos=  type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Static 
                ).Where(query =>  query.GetCustomAttribute(typeof(CommandAtrribute)) is not null ); 

                foreach (MethodInfo method  in methdos) {
                    
                    #pragma warning disable  CS8600
                    CommandAtrribute atrributeInstance = method.GetCustomAttribute(typeof(CommandAtrribute)) as CommandAtrribute; 
                    #pragma warning disable 
                    commands_register.Add(atrributeInstance.Command , method);
                }

            }
            return commands_register ;
            
        }

    }   


}