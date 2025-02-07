
public class CommandAttribute(string callName_, string help_ ="No help") : Attribute
{
    public  string Command  { get => callName_; private set { } }
    public  string Help {get => help_ ;  private set{} }
}