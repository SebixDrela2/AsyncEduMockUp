using System.Runtime.CompilerServices;

namespace AsyncEduMockUp.Utils;

public enum LoggingLevel
{
    Debug,
    Verbose,
    Info,
    Error
}

internal static class Logger
{
    private const LoggingLevel Level = LoggingLevel.Info;

    public static void LogDebug(string message, [CallerMemberName] string origin = "") => Log(LoggingLevel.Debug, message ,origin);
    public static void LogVerbose(string message, [CallerMemberName] string origin = "") => Log(LoggingLevel.Verbose, message ,origin);
    public static void LogInfo(string message, [CallerMemberName] string origin = "") => Log(LoggingLevel.Info, message ,origin);
    public static void LogError(string message, [CallerMemberName] string origin = "") => Log(LoggingLevel.Error, message ,origin);

    private static void Log(
        LoggingLevel loggingLevel, 
        string message, 
        [CallerMemberName] string origin = "")
    {
        if (loggingLevel < Level)
        {
            return;
        }

        Console.WriteLine($"[{origin,15}] {message} ThreadID:{Environment.CurrentManagedThreadId}");
    }
}
