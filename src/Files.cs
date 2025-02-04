#pragma warning disable IDE0130

using System.Runtime.CompilerServices;

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
                    if (isDirectory) CopyDirectory(target, destination, true); // TODO   copy dir call;
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

            bool replaceAll  =  default ; 

            foreach (string file in files)
            {
                string currentFileDestination = Path.Combine(destination, file);
                if(File.Exists(currentFileDestination) && overwrite){
                    if(!replaceAll)
                    Log.AskFor($"Overwrite existing files? (current:{file})", ()=> replaceAll = true , ()=>  replaceAll =  false) ;
                    else{
                        File.Copy(file, currentFileDestination) ;
                    }
                }
            }
            Log.Error("Something went wrong");
            return default;
        }
    }
}