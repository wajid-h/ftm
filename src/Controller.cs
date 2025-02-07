using VCS.FileControllers;

namespace VCS.Core
{
    public class Controller
    {


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

                SETTINGS.STAGE_PATH = stageDir.FullName;

            }

            SETTINGS.ROOT = initialPath;
            SETTINGS.VERSIONS_ROOT = versionsPath;
            SETTINGS.STAGE_PATH = stagePath;
        }

     
        public static bool Stage(string[] paths)
        {
            if (paths.Length is 0) return false;

            foreach (string path in paths)
            {
                string dest = Path.Combine(SETTINGS.STAGE_PATH, path);

                PathType type = FileUtils.IdentifyPathType(path);
                return type == PathType.File ?
                FileMover.CopyFile(path, SETTINGS.STAGE_PATH, SETTINGS.BACKUP_EXTENSION_MARK, ExtensionMode.Append, true) :
                FileMover.CopyDirRecursive(path, dest, true, ExtensionMode.Append);

            }
                        
            return true;
        }
  
        public static bool Destage(params string[] paths)
        {
            if (paths.Length is 0) return false;

            foreach (string path in paths)
            {

                // File.cs  -> File.cs.bak
                PathType type = FileUtils.IdentifyPathType(path);
                return type == PathType.File ?
                FileMover.RemoveFile(Path.Combine(SETTINGS.STAGE_PATH, path + SETTINGS.BACKUP_EXTENSION_MARK)) :
                FileMover.RemoveDirectory(Path.Combine(SETTINGS.STAGE_PATH, path));
            }
            return true;
        }

        public static bool Versionize()
        {

            bool done = FileUtils.DirSecureHash(SETTINGS.STAGE_PATH, make_version);

            static void make_version(string versionName)
            {
                Console.WriteLine(versionName);
                FileMover.CopyDirRecursive(SETTINGS.STAGE_PATH, Path.Combine(SETTINGS.VERSIONS_ROOT, versionName), true, ExtensionMode.Perserve);
            }
            return done;
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