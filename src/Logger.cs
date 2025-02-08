public class Log
{
    public class TUIAffirmCancelRequest
    {   
        /// <summary>
        /// Creates a simple TUI yes/no prompt, performs action respective of user response.
        /// </summary>
        /// <param name="message">the prompt</param>
        /// <param name="onYes">action on affirm</param>
        /// <param name="onNo">action on negative</param>
        public TUIAffirmCancelRequest(string message, Action onYes, Action onNo)
        {

            Log.Write(message + " (Y/N)");
            string? readValue = Console.ReadLine();
            if (string.IsNullOrEmpty(readValue) || readValue.Equals("n", StringComparison.OrdinalIgnoreCase)){
                onNo.Invoke();
                return;
            }
            else if (readValue.Equals("y", StringComparison.OrdinalIgnoreCase)){
                onYes.Invoke();
                return;
            }
            else{
                onNo.Invoke();
                return;
            }
        }


    }



    public class LogSettings(LoggingLevel level_)
    {
        /// set some stuff up 
        // logging level [verbose, debug, warrning, error , or sum shi]
        public LoggingLevel Level { get => level; }

        private readonly LoggingLevel level = level_;

    }

    [Flags]
    public enum LoggingLevel
    {
        Debug,
        Verbose,
        Information,
        Warning,
        Error,
        Fatal
    }
    public static LogSettings? Settings { get; set; }



    public static void Error(string err)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteLog(LoggingLevel.Error, err);

    }
    public static void Warning(string warn)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteLog(LoggingLevel.Warning, warn);
    }
    public static void Debug(string dbgMessage)
    {
        Console.ForegroundColor = ConsoleColor.Green;

        WriteLog(LoggingLevel.Debug, dbgMessage);
    }
    public static void Info(string inf)
    {
        Console.ForegroundColor = ConsoleColor.White;

        WriteLog(LoggingLevel.Information, inf);
    }


    public static void Fatal(string fatality)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        WriteLog(LoggingLevel.Fatal, fatality);
    }

    public static void Write(string s) => Console.Write(s);
    public static void WriteLine(string s) => Console.WriteLine(s);

    public static void Verbose(string verbose, bool padstart = default, bool padend = default)
    {
        if (padstart) Console.Write("\n");
        WriteLog(LoggingLevel.Verbose, verbose);
        if (padend) Console.Write("\n");
    }
    public static TUIAffirmCancelRequest AskFor(string message, Action onYes, Action onNo)
    {
        return new(message, onYes, onNo);
    }

    /// <summary>
    /// Writes a log to console, with type of log and timestamp
    /// </summary>
    /// <param name="logLevel">log type</param>
    /// <param name="message">message</param>
    private static void WriteLog(LoggingLevel logLevel, string message)
    {

        if (Settings == null)
        {   
            Settings = new(LoggingLevel.Debug);
            
        }


        if ((int)logLevel < (int)Settings.Level)
            return;

        DateTime timeStamp = DateTime.Now;
        string typeSuffix = logLevel.ToString();
        string logMessageTimeStamp = $"{timeStamp.Hour: 00}:{timeStamp.Minute:00}:{timeStamp.Second:00}";
        string logMessageEntry = $"[{logMessageTimeStamp} - {typeSuffix}]:{message}";
        string logFileEntry = $"[{timeStamp} - {typeSuffix}] {message}";
        Console.WriteLine(logMessageEntry);
        Console.ResetColor();
    }

}