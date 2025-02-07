#pragma warning disable IDE0130
using System.Security.Cryptography;

namespace VCS.FileControllers
{
#pragma warning restore

    public enum PathType { File, Directory }
    public enum ExtensionMode { Append, Remove, Perserve }
    internal enum PutType { Copy, Move }

    public class FileMover
    {

        /// <summary>
        /// uses internal Put subroutine to move file from source to dest.
        /// </summary>
        /// <param name="path">path of file</param>
        /// <param name="destination">path to destination file</param>
        /// <param name="mode">the extension mode</param>
        /// <param name="overwrite">overwrite the file?</param>
        /// <returns></returns>
        public static bool MoveFile(string path, string destination, ExtensionMode mode, bool overwrite = false) => Put(path, destination, PathType.File, mode, overwrite);
        public static bool MoveDirectory(string path, string destination, ExtensionMode mode) => Put(path, destination, PathType.Directory, mode);

        /// <summary>
        /// Put file from  source to destination.
        /// </summary>
        /// <param name="target">the target file</param>
        /// <param name="destination">destination location</param>
        /// <param name="type">path type, dir or file.</param>
        /// <param name="mode">exension mode</param>
        /// <param name="overwrite">wheter to overrwrite the files</param>
        /// <param name="put">put type, put as a copy or move the file?</param>
        /// <returns></returns>
        static internal bool Put(string target, string destination, PathType type, ExtensionMode mode, bool overwrite = false, PutType put = PutType.Move)
        {
            try
            {
                bool isDirectory = type == PathType.Directory;
                bool exists = !isDirectory ? File.Exists(target) : Directory.Exists(target);

                if (!exists) return default;

                if (put == PutType.Move)
                {
                    if (isDirectory) Directory.Move(target, destination);
                    else File.Move(target, destination, overwrite);
                }
                else
                {
                    if (isDirectory) CopyDirRecursive(target, destination, overwrite, mode);
                    else File.Copy(target, destination, overwrite);
                }
                return true;
            }
            catch (UnauthorizedAccessException) { throw; }
            catch (AccessViolationException) { throw; }
            catch (InvalidOperationException) { throw; }
        }



        /// <summary>
        /// Copies file to destination 
        /// </summary>
        /// <param name="filePath">the subject file</param>
        /// <param name="destination">destination</param>
        /// <param name="fileExtension">extension to apply / remove when placing new file</param>
        /// <param name="mode">remove/append extension</param>
        /// <param name="replace">replace if the file already exists at destination?</param>
        /// <returns>true on action success</returns>
        static internal bool CopyFile(string filePath, string destination, string fileExtension, ExtensionMode mode, bool replace)
        {
            if (!File.Exists(filePath))
                return false;

            FileInfo file = new(filePath);
            string originalFileName = file.Name;

            bool isDestDirectory = FileUtils.IdentifyPathType(destination) == PathType.Directory;

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
        /// <summary>
        /// Applies/Removes given extension to or from a file 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode">Remove/Apply</param>
        /// <param name="extension">The Extension</param>
        /// <returns>filename with new extension </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static string ApplyExtensionMode(string fileName, ExtensionMode mode, string extension)
        {
            return mode switch
            {
                ExtensionMode.Perserve => fileName,
                ExtensionMode.Append => fileName + extension,
                ExtensionMode.Remove => fileName.EndsWith(extension, StringComparison.Ordinal)
                                        ? fileName[..^extension.Length]
                                        : fileName,

                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
        }

        /// <summary>
        /// recurvisly copies directory from top to bottom, over to destination.
        /// </summary>
        /// <param name="directory">The directory to copy</param>
        /// <param name="destination">The destination directory</param>
        /// <param name="overwrite">Whether to overrite files that exist in destination beforehead</param>
        /// <param name="mode">The extension mode</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="AccessViolationException"></exception>
        /// <returns>true if action succeded</returns>
        static internal bool CopyDirRecursive(string directory, string destination, bool overwrite, ExtensionMode mode)
        {
            try
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
                return true;
            }
            catch (AccessViolationException) { return false; throw; }
            catch (UnauthorizedAccessException) { return false; throw; }

        }


        /// <summary>
        /// Deletes the object at given path (file or directory) be fucking cautious with ts
        /// </summary>
        /// <param name="path">path to remove</param>
        /// <returns>true on success</returns>
        static internal bool RemovePath(string path)
        {
            PathType type = FileUtils.IdentifyPathType(path);

            if (type == PathType.File) return RemoveFile(path);
            return RemoveDirectory(path);
        }



        /// <summary>
        /// Removes directory, reports back status, prefer RemovePath over this.
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns>true if directory was removed.</returns>
        static internal bool RemoveDirectory(string dirPath)
        {
            try
            {
                DirectoryInfo dir = new(dirPath);
                if (!dir.Exists) return false;
                dir.Delete(true);
                return true;
            }
            catch (DirectoryNotFoundException) { return default; }
            catch (Exception) { return default; }
        }
        /// <summary>
        /// Removes file and reports the action's success, prefer RemovePath over this.
        /// </summary>
        /// <param name="filePath">File to remove</param>
        /// <returns>true if the file was removed, false if file does not exist or was not removed</returns>
        static internal bool RemoveFile(string filePath)
        {
            try
            {
                FileInfo file = new(filePath);
                if (!file.Exists) return false;
                file.Delete();
                return true;
            }
            catch (FileNotFoundException) { return false; }
            catch (Exception) { return false; }
        }
    }


    public class FileUtils
    {

        /// <summary>
        /// Gets type of object at a path, i.e a File / Directory
        /// </summary>
        /// <param name="path"></param>
        /// <param name="priority"></param>
        /// <returns>PathType object that represents the type of object at path</returns>
        internal static PathType IdentifyPathType(string path, PathType priority = PathType.File)
        {

            bool isFile = File.Exists(path);
            bool isDir = Directory.Exists(path);

            if (isDir && isFile)
                return priority;

            return isDir ? PathType.Directory : PathType.File;

        }

        /// <summary>
        /// Computes a SHA-1 hash of a directory and performs an optionally given action on completion.
        /// do not use it as in actual security context, purpose here is to just make sure we always have unique versions.
        /// </summary>
        /// <param name="dirPath">the target directory</param>
        /// <param name="onFinished">the action on completion</param>
        /// <returns>true if hashing process was completed.</returns>
        public static bool DirSecureHash(string dirPath, Action<string>? onFinished = null)
        {

            DirectoryInfo dir = new(dirPath);
            string tempFilePath = Path.GetTempPath();

            if (!dir.Exists)
            {
                Log.Error(dir.FullName + " Does not exist.");
                return false;
            }

            string tempFileName = Utils.GetRandomString(10);
            Log.WriteLine(tempFileName);

            using Stream tempFileStream = File.Open(Path.Combine(tempFilePath, tempFileName), FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);

            FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);


            SHA1 hasher = SHA1.Create();
            foreach (FileInfo file in files)
            {

                Stream fileRead = File.OpenRead(file.FullName);
                tempFileStream.Write(hasher.ComputeHash(fileRead));
            }

            // reset the damm streaam back to zero to calculate the hash from beggining otherwise ts reads an empty buffer for hasing
            tempFileStream.Seek(0, SeekOrigin.Begin);

            byte[] finalHash = hasher.ComputeHash(tempFileStream);

            string hash = BitConverter.ToString(finalHash).ToLower().Replace("-", "");


            // release dat file first
            tempFileStream.Flush();
            tempFileStream.Dispose();
            tempFileStream.Close();


            FileInfo tempFile = new(Path.Combine(tempFilePath, "hash.temp"));

            tempFile.Delete();
            onFinished?.Invoke(hash);
            return true;

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