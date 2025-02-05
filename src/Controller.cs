using System.Diagnostics;
using System.Dynamic;
using FTM.FileControllers;

namespace FTM.Core
{
    public class Controller
    {
        internal enum PathType { File, Directory }

        static DirectoryInfo? baseDir;
        static DirectoryInfo? stageDir;
        static DirectoryInfo? versionsDir;

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
                Log.WriteLine("Initializing repository....");
                baseDir = Directory.CreateDirectory(initialPath);
                stageDir = Directory.CreateDirectory(stagePath);
                versionsDir = Directory.CreateDirectory(versionsPath);

                SETTINGS.STAGE_PATH =  stageDir.FullName ;

            }
        }


        public static bool Stage(string path)
        {
            string dest = Path.Combine(SETTINGS.STAGE_PATH, path); 
        
            PathType type = IdentifyType(path);
            return type == PathType.File ?
            FileMover.CopyFile(path, SETTINGS.STAGE_PATH , SETTINGS.BACKUP_EXTENSION_MARK , FileMover.ExtensionMode.Append , true) :
            FileMover.CopyDirectory(path,dest , true, FileMover.ExtensionMode.Append) ;   

        }

        private static PathType IdentifyType(string path, PathType priority = PathType.File)
        {

            bool isFile = File.Exists(path);
            bool isDir = Directory.Exists(path);

            if (isDir && isFile)
                return priority;

            return isDir ? PathType.Directory : PathType.File;

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