using AsyncEduMockUp.Core;
using AsyncEduMockUp.Setup;
using AsyncEduMockUp.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        var taskList = new List<EduTask<CharHashCode>>();

        for (var c = 'A'; c <= 'Z'; ++c)
        {
            var chor = c;
            var task = EduTask.Run(() => Setup.GetHashCodeFromMemberCXor(chor));

            taskList.Add(task);
        }

        foreach(var task in taskList)
        {
            task.ContinueWith((result) => Logger.LogDebug($"Task completed: {result}"));
        }

        Thread.Sleep(5000);

        var completedTasks = taskList
            .Where(x => x.IsCompleted)
            .ToArray();

        foreach (var task in completedTasks)
        {
            Logger.LogInfo($"{task.Result.C} {task.Result.HashCode}");
        }

        EduThreadPool.Default.Dispose();
    }
}