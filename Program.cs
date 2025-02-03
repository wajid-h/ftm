using FTM.FileControllers;
namespace FTM
{
    public class FTMCore
    {

        // entry
        public static void Main(params string[] args)
        {

            FileMover.CopyDirectory(args[0], args[1], true);

            static void resume()   => Log.WriteLine("You said yes, lets continue.");
            static void halt() => Log.WriteLine("You said no, lets go.");

            Log.AskFor("Do you want to continue?", resume, halt);
        }

    }
}