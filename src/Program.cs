using FTM.FileControllers;
namespace FTM
{
    public class FTMCore
    {
        // entry
        public static void Main(params string[] args)
        {
            FileMover.CopyDirectory(args[0], args[1], true);
        }

    }
}