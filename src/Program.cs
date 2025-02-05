using FTM.Core;
namespace FTM
{
    public class FTMCore
    {
        // entry
        public static void Main(params string[] args)
        {
            Controller.Init(".");
            Controller.Stage(args[0]);
            
            Log.AskFor("Continue?", () => {} , () =>{});
            Controller.Versionize();         
            
            // FileMover.CopyDirectory
            // (path, Path.Join("yo", path),
            //  true, FileMover.ExtensionMode.Remove) ;   
        }

    }
}