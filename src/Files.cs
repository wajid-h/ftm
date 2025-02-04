#pragma warning disable IDE0130
using System.Diagnostics;
using System.Dynamic;
using System.Text;
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

            CopyDirRecursive(directory, destination,  overwrite, mode);
            return default; 

        }


        static private void CopyDirRecursive(string directory, string destination, bool overwrite, ExtensionMode mode){

            DirectoryInfo sourceDir  =  new(directory) ;
            if(!sourceDir.Exists) 
            throw new DirectoryNotFoundException($"Dir {directory} not found");

            DirectoryInfo  dest =Directory.CreateDirectory(destination) ;

            DirectoryInfo[] dirs =  sourceDir.GetDirectories();
            
            foreach(FileInfo file in sourceDir.GetFiles())  {
                string targetFilePath = Path.Combine(dest.FullName, file.Name) ; 
                file.CopyTo( targetFilePath,overwrite );
            }
            foreach (DirectoryInfo subDir in dirs){
                string newDestDir =  Path.Combine(destination ,subDir.Name );
                CopyDirRecursive(subDir.FullName, newDestDir, overwrite,  mode);
            }        
        }

    }
}