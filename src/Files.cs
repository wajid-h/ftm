#pragma warning disable IDE0130


using System.Diagnostics;
using System.Text;

namespace FTM.FileControllers
{
#pragma warning restore


    public class FileMover
    {
        public enum MoveType { File, Directory }
        internal enum PutType { Copy, Move }

        public static bool MoveFile(string path, string destination, bool overwrite = false) => Put(path, destination, MoveType.File, overwrite);
        public static bool MoveDirectory(string path, string destination) => Put(path, destination, MoveType.Directory);

        static internal bool Put(string target, string destination, MoveType type, bool overwrite = false, PutType put = PutType.Move)
        {
            try
            {
                bool isDirectory = type == MoveType.Directory;
                bool exists = !isDirectory ? File.Exists(target) : Directory.Exists(target);

                if (!exists) return default;

                if (put == PutType.Move)
                {
                    if (isDirectory) Directory.Move(target, destination);
                    else File.Move(target, destination, overwrite);
                }
                else
                {
                    if (isDirectory) CopyDirectory(target, destination, overwrite);
                    else File.Copy(target, destination, overwrite);
                }
                return true;
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (AccessViolationException) { throw; }
            catch (InvalidOperationException) { throw; }
        }

        static internal bool CopyDirectory(string directory, string destination, bool overwrite)
        {
            // get all files
            string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);

            bool replaceDirs = false;
            bool replaceFiles = false;

            foreach (string dir in dirs)
            {
                string newDirPath = Path.Combine(destination, dir);
                if (Directory.Exists(newDirPath) && replaceDirs == false){
                    Log.AskFor($"Directoryy '{newDirPath}' already exists, replace conflicting dirs?", () => replaceDirs = true, () => replaceDirs = false);
                    if (replaceDirs) Directory.CreateDirectory(Path.Combine(destination, dir));
                }
                else Directory.CreateDirectory(Path.Combine(destination, dir));
            }
            foreach (string file in files)
            {
                string filePath = Path.Combine(destination, file + ".bak");
                if (File.Exists(filePath) && !replaceFiles){
                    Log.AskFor($"File '{filePath}' already exists, replace conflicting files?", () => replaceFiles = true, () => replaceDirs = false);

                    if (replaceFiles)  File.Copy(file, filePath, overwrite);
                }
                else File.Copy(file, filePath, overwrite);

            }
            return default;
        }

        static internal string GetParentByDir(string basePath)
        {
            List<string> pathContent = [.. basePath.Split(Path.DirectorySeparatorChar)];
            pathContent.RemoveAt(pathContent.Count - 1);

            StringBuilder pathBuilder = new();
            foreach (string pathStrand in pathContent)
            {
                pathBuilder.Append(pathStrand);
            }
            return pathBuilder.ToString();
        }
    }
}