#pragma warning disable IDE0130
namespace FTM.FileControllers
{
#pragma warning restore


    public class FileMover
    {
        public enum MoveType { File, Directory }
        public enum ExtensionMode { Append, Remove }

        internal enum PutType { Copy, Move }

        public static bool MoveFile(string path, string destination, ExtensionMode mode, bool overwrite = false) => Put(path, destination, MoveType.File, mode, overwrite);
        public static bool MoveDirectory(string path, string destination, ExtensionMode mode) => Put(path, destination, MoveType.Directory, mode);

        static internal bool Put(string target, string destination, MoveType type, ExtensionMode mode, bool overwrite = false, PutType put = PutType.Move)
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
                    if (isDirectory) CopyDirectory(target, destination, overwrite, mode);
                    else File.Copy(target, destination, overwrite);
                }
                return true;
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (AccessViolationException) { throw; }
            catch (InvalidOperationException) { throw; }
        }


        static internal bool CopyDirectory(string directory, string destination, bool overwrite, ExtensionMode mode)
        {

            CopyDirRecursive(directory, destination, overwrite, mode);
            return default;

        }

        static internal bool CopyFile(string filePath, string destination, string fileExtension, ExtensionMode mode, bool replace)
        {
            if (!File.Exists(filePath))
                return false;

            FileInfo file = new(filePath);
            string originalFileName = file.Name;

            // Determine if the destination is intended to be a directory
            bool isDestDirectory = destination.EndsWith(Path.DirectorySeparatorChar.ToString())
                                   || destination.EndsWith(Path.AltDirectorySeparatorChar.ToString())
                                   || Directory.Exists(destination);

            string destDir;
            string destFile;

            if (isDestDirectory)
            {
                // Treat destination as a directory: Use original filename with extension modification
                destDir = destination.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                destFile = ApplyExtensionMode(originalFileName, mode, fileExtension);
            }
            else
            {
                // Treat destination as a full file path: Modify the filename part
                destDir = Path.GetDirectoryName(destination)!;
                string targetFileName = Path.GetFileName(destination);
                destFile = ApplyExtensionMode(targetFileName, mode, fileExtension);
            }

            string copyingTo = Path.Combine(destDir, destFile);
            Directory.CreateDirectory(destDir); // Ensure the target directory exists

            file.CopyTo(copyingTo, replace);
            return true;
        }

        private static string ApplyExtensionMode(string fileName, ExtensionMode mode, string extension)
        {
            return mode switch
            {
                ExtensionMode.Append => fileName + extension,
                ExtensionMode.Remove => fileName.EndsWith(extension, StringComparison.Ordinal)
                                        ? fileName[..^extension.Length]
                                        : fileName,
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
        }
        static private void CopyDirRecursive(string directory, string destination, bool overwrite, ExtensionMode mode)
        {

            DirectoryInfo sourceDir = new(directory);
            if (!sourceDir.Exists)
                throw new DirectoryNotFoundException($"Dir {directory} not found");

            DirectoryInfo dest = Directory.CreateDirectory(destination);

            DirectoryInfo[] dirs = sourceDir.GetDirectories();

            foreach (FileInfo file in sourceDir.GetFiles())
            {
                string targetFilePath = Path.Combine(dest.FullName, file.Name);

                CopyFile(file.FullName, targetFilePath, SETTINGS.BACKUP_EXTENSION_MARK, mode, true);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestDir = Path.Combine(destination, subDir.Name);
                CopyDirRecursive(subDir.FullName, newDestDir, overwrite, mode);
            }
        }


        static internal bool RemoveDirectory(string dirPath)
        {
            DirectoryInfo dir = new(dirPath);
            if (!dir.Exists) return false;
            dir.Delete(true);
            return true;
        }
        static internal bool RemoveFile(string filePath)
        {

            FileInfo file = new(filePath);
            if (!file.Exists) return false;
            file.Delete();
            return true;
        }
    }
}