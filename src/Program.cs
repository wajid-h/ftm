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

            Task.Delay(100);
            Controller.Stage(args[0]);
        }

    }
}