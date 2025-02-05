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
           // Controller.Init(".");
            Controller.Destage(args[0]);
         
            //    FileMover.CopyDirectory
            // (path, Path.Join("yo", path),
            //  true, FileMover.ExtensionMode.Remove) ;   
        }

    }
}