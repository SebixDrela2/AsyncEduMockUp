using System.Runtime.CompilerServices;

namespace AsyncEduMockUp.Utils;

internal static class Logger
{
    public static void Log(string message, [CallerMemberName] string origin = "") 
        => Console.WriteLine($"[{origin, 15}] {message} ThreadID:{Environment.CurrentManagedThreadId}");
}
