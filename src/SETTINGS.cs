#pragma warning disable CA1050
public static class SETTINGS{
#pragma warning restore

    public static  string ROOT  = ".vcs"; 
    public static  string VERSIONS_ROOT= Path.Combine(ROOT , ".versions") ;
    
    public static  string STAGE_PATH =  Path.Combine(ROOT, ".stage");
    public static readonly string BACKUP_EXTENSION_MARK = ".bak"; 

}