using AsyncEduMockUp.Core;
using AsyncEduMockUp.Setup;
using AsyncEduMockUp.Utils;
using System.Threading.Tasks;

internal class Program
{
    private static void Main(string[] args)
    {
        RunCharHashCode().Wait();

        EduThreadPool.Default.Dispose();
    }

    private static async Task RunCharHashCode()
    {
        var taskList = new List<EduTask<CharHashCode>>();

        for (var c = 'A'; c <= 'Z'; ++c)
        {
            var chor = c;
            var task = EduTask.Run(() => Setup.GetHashCodeFromMemberCXor(chor));

            taskList.Add(task);
        }

        var continuations = taskList
            .Select(task => task.ContinueWith((result) => Logger.LogDebug($"Task completed: {result}")));

        await EduTask.WhenAll(continuations);

        var completedTasks = taskList
            .Where(x => x.IsCompleted)
            .ToArray();

        foreach (var task in completedTasks)
        {
            Logger.LogInfo($"{task.Result.C} {task.Result.HashCode}");
        }
    }
}