using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FTM.Core
{
    public class Controller
    {
        public static void Init(string directory)
        {
            if (!Directory.Exists(directory))
            {
                STD_ERROR.DirectoryNotPresent(directory);
                return;
            }

            string initialPath = Path.Combine(directory, SETTINGS.ROOT);
            string stagePath = Path.Combine(directory, SETTINGS.STAGE_PATH);
            string versionsPath = Path.Combine(directory, SETTINGS.VERSIONS_ROOT);

            if (!HasMissingDirs(initialPath, stagePath, versionsPath))
            {
                Log.WriteLine($"Using existing vcs instance at {initialPath}");
            }
            else
            {
                Log.WriteLine("Initializing repository....") ;
                Directory.CreateDirectory(initialPath);
                Directory.CreateDirectory(stagePath);
                Directory.CreateDirectory(versionsPath);
                
            }
        }

        internal static bool HasMissingDirs(params string[] dirs)
        {
            bool missing_dirs = false;

            foreach (string dir in dirs)
            {

                missing_dirs = Directory.Exists(dir) is false;
                if (missing_dirs)
                    break;
            }
            return missing_dirs;
        }
    }

}