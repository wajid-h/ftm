
public class CommandAtrribute(string callName_, string help_) : Attribute
{
    public  string Command  { get => callName_; private set { } }
    public  string Help {get => help_ ;  private set{} }
}