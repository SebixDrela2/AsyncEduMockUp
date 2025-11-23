using AsyncEduMockUp.Core;
using AsyncEduMockUp.Setup;
using AsyncEduMockUp.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        var taskList = new List<EduTask<int>>();

        for (var c = 'A'; c <= 'A'; ++c)
        {
            var chor = c;
            var task = EduTask.Run(() => Setup.GetHashCodeFromMemberCXor(chor));

            taskList.Add(task);
        }

        foreach(var task in taskList)
        {
            task.ContinueWith((result) => Logger.Log($"Task completed: {result}"));
        }

        while(true)
        {
            Thread.Sleep(5000);

            var completedTasks = taskList
                .Where(x => x.IsCompleted)
                .ToArray();

            foreach(var task in completedTasks)
            {
                Logger.Log($"{task.Result}");
            }
        }
    }
}