using System;

public class CommandAtrribute(
string callName_, string help_
): Attribute {

 
public readonly string Command 
{get=> callName_; private set{}};

public readonly string Help
{get=> help_; private set{};}

}