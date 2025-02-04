#pragma warning disable CA1050
public static class SETTINGS{
#pragma warning restore

    public static readonly string ROOT  = ".vcs"; 
    public static readonly string VERSIONS_ROOT= Path.Combine(ROOT , ".versions") ;
    
    public static readonly string STAGE_PATH =  Path.Combine(ROOT, ".stage");
    public static readonly string BACKUP_EXTENSION_MARK = ".bak"; 

}