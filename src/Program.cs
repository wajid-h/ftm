using System.Diagnostics.Contracts;
using FTM.Core;
using FTM.FileControllers;
namespace FTM
{
    public class FTMCore
    {
        // entry
        public static void Main(params string[] args)
        {
            Controller.Init(".");
            Controller.Stage(args[0]);
            string path = ".vcs\\.stage\\bin" ;
            //    FileMover.CopyDirectory
            // (path, Path.Join("yo", path),
            //  true, FileMover.ExtensionMode.Remove) ;   
        }

    }
}