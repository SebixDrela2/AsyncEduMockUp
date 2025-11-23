using AsyncEduMockUp.Core;
using AsyncEduMockUp.Setup;
using AsyncEduMockUp.Utils;
using System.Threading.Tasks;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await RunCharHashCode();
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

        foreach (var task in taskList)
        {
            Logger.LogInfo($"{task.Result.C} {task.Result.HashCode}");
        }
    }
}