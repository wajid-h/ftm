#pragma warning disable
public static class STD_ERROR {
#pragma warning restore
    public static void DirectoryNotPresent(string  s) =>  Log.Error($"Directory '{s}' does not exist.") ;
    public static void FileNotPresent(string  s) =>  Log.Error($"Filee '{s}' does not exist.") ;
    
}