using System.Reflection;
namespace VCS 
{

    public class CommandRegistery{

        private  Dictionary<string, MethodInfo>? commands ;
        public  Dictionary<string, MethodInfo>  Commands {get =>  commands ??= LoadRegistery();  }

        // new file
        private static Dictionary<string, MethodInfo> LoadRegistery(){

            Dictionary<string, MethodInfo> commands_register=  [] ; 

            Assembly  executingAssembly =  Assembly.GetExecutingAssembly();

            Type[]  activeTypes = executingAssembly.GetTypes();


            foreach (Type type  in activeTypes){

                var  methdos=  type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Static 
                ).Where(query =>  query.GetCustomAttribute(typeof(CommandAttribute)) is not null ); 

                foreach (MethodInfo method  in methdos) {
                    
                    #pragma warning disable  CS8600
                    CommandAttribute atrributeInstance = method.GetCustomAttribute(typeof(CommandAttribute)) as CommandAttribute; 
                    #pragma warning disable 
                    commands_register.Add(atrributeInstance.Command , method);
                }

            }
            return commands_register ;
            
        }

    }   


}