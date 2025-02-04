#pragma warning disable IDE0130
using System.Text;
namespace FTM.FileControllers
{
#pragma warning restore


    public class FileMover
    {
        public enum MoveType { File, Directory }
        public enum ExtensionMode {Append, Remove}


        internal enum PutType { Copy, Move }
        
        public static bool MoveFile(string path, string destination, ExtensionMode mode, bool overwrite = false) => Put(path, destination, MoveType.File, mode, overwrite);
        public static bool MoveDirectory(string path, string destination, ExtensionMode mode) => Put(path, destination, MoveType.Directory , mode);

        static internal bool Put(string target, string destination, MoveType type,  ExtensionMode mode,bool overwrite = false, PutType put = PutType.Move)
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
            // get all files
            string[] files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);

            bool replaceDirs = false;
            bool replaceFiles = false;

            foreach (string dir in dirs)
            {
                string newDirPath = Path.Combine(destination, dir);
                if (Directory.Exists(newDirPath) && replaceDirs == false)
                {
                    Log.AskFor($"Directoryy '{newDirPath}' already exists, replace conflicting dirs?", () => replaceDirs = true, () => replaceDirs = false);
                    if (replaceDirs) Directory.CreateDirectory(Path.Combine(destination, dir));
                }
                else Directory.CreateDirectory(Path.Combine(destination, dir));
            }
            foreach (string file in files)
            {
                string filePath = Path.Combine(destination, file + ".bak");
                if (File.Exists(filePath) && !replaceFiles)
                {
                    Log.AskFor($"File '{filePath}' already exists, replace conflicting files?", () => replaceFiles = true, () => replaceDirs = false);

                    if (replaceFiles) CopyFile(file, filePath, overwrite, SETTINGS.BACKUP_EXTENSION_MARK, mode);
                }
                else CopyFile(file, filePath, overwrite, SETTINGS.BACKUP_EXTENSION_MARK, mode);

            }
            return default;
        }


        static internal bool CopyFile(string file, string destination, bool overwrite, string extension , ExtensionMode mode)
        {   
           
            if(!File.Exists(file)) return false;
             
            string fileName =  Path.GetFileName(file);  

            if(mode ==  ExtensionMode.Remove)   
            fileName = fileName.Replace(".bak", string.Empty) ;
            else fileName += ".bak" ;
            
            string destinationFileName = Path.Combine(destination, fileName);
            
            File.Copy(file, destinationFileName, overwrite) ;
          
            return  true ;
        }
    }
}