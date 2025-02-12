using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VCS.FileControllers;

namespace VCS.Core
{
    public class Controller
    {


        static DirectoryInfo? baseDir;
        static DirectoryInfo? stageDir;
        static DirectoryInfo? versionsDir;
        /// <summary>
        /// Initializes a repo at given path, creates folder structure if not available already
        /// </summary>
        /// <param name="directory">the target directory to init in</param>
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

            string versionHistory = Path.Combine(SETTINGS.ROOT, SETTINGS.VERSION_HISTORY_FILENAME);
            if (!File.Exists(versionHistory))
                File.Create(versionHistory);
        }

        /// <summary>
        /// puts a copy of files in paths into stage 
        /// </summary>
        /// <param name="paths">files to stage</param>
        /// <returns>true if success</returns>
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

        /// <summary>
        /// removes files from stage
        /// </summary>
        /// <param name="paths"></param>
        /// <returns>true if successfully removes all file</returns>
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
        /// <summary>
        /// Computes a SHA-1 of current stage, copy the content of stage over to a new version 
        /// </summary>
        /// <returns>true on success</returns>
        public static bool Versionize()
        {

            bool done = FileUtils.DirSecureHash(SETTINGS.STAGE_PATH, make_version);

            static void make_version(string versionName)
            {
                Console.WriteLine(versionName);
                FileMover.CopyDirRecursive(SETTINGS.STAGE_PATH, Path.Combine(SETTINGS.VERSIONS_ROOT, versionName), true, ExtensionMode.Perserve);

                write_changes(versionName) ;
            }

            static void write_changes(string fileHash)
            {

                string infoPath = Path.Combine(SETTINGS.ROOT, SETTINGS.VERSION_HISTORY_FILENAME);

                StringBuilder newEntry = new();

                byte[] lastLineBuffer = [];

                // changed line, hash should be diffferent...

                int lastVersion ;
                // read the last version
                List<string> lines =[.. File.ReadLines(infoPath)] ;
                if(lines.Count == 0 ) 
                {
                    lastVersion =  0 ;
                }
                else{
                bool parsedLastVersion = int.TryParse(lines.Last().Replace(" ", "").Split("-")[0], out  lastVersion);
                }
                
                newEntry.Append($"{lastVersion + 1 }-{DateTime.Now}-{fileHash}");
                
                using StreamWriter writer = File.AppendText(infoPath);
                writer.WriteLine(newEntry.ToString());
            }
            return done ;

        }



        /// <summary>
        /// walks through a list of paths and checks if all of them are present
        /// </summary>
        /// <param name="dirs">the dirs to walk throughh</param>
        /// <returns>true if one or more directories are missing from 'dirs'</returns>
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