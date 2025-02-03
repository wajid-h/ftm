using System.Text;

namespace FTM.FileController
{
    public class FileMover
    {
        public enum MoveType { File, Directory }
        public static bool MoveFile(string file, string destination, bool overwrite = false) =>  Move(file  , destination, MoveType.File , overwrite) ;
        public static void MoveDir(string file, string destination) => Move(file, destination , MoveType.Directory) ; 

        static internal bool Move(string target, string destination, MoveType type,  bool overwrite = false  )
        {   
            try 
            {   
                bool isDirectory   = type == MoveType.Directory ;
                bool exists =    !isDirectory ?  File.Exists(target) :  Directory.Exists(target) ; 
                
                if (!exists) return default;

                if(isDirectory)
                Directory.Move(target, destination);
                else  File.Move(target, destination , overwrite) ;
                
                return true;
            }
            catch (UnauthorizedAccessException) { return default; }
            catch (AccessViolationException) { return default; }
            catch (InvalidOperationException) { return default; }
        }

    }

    public class FileUtils
    {
            
    }
}