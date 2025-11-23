using AsyncEduMockUp.Utils;
using System.Collections.Concurrent;

namespace AsyncEduMockUp.Core;

internal class EduThreadPool(Thread[] workers)
{
    private readonly Thread[] _workers = workers;
    private readonly ConcurrentBag<IEduThreadPoolItem> _tasks = [];

    private static readonly EduThreadPool _default = Create(1);
    public static EduThreadPool Default => _default;

    public static EduThreadPool Create(int threadCount)
    {
        var workers = new Thread[threadCount];
        var instance = new EduThreadPool(workers);

        foreach(ref var worker in workers.AsSpan())
        {
            worker = new Thread(instance.StartWork);
            worker.Start();
        }

        return instance;
    }

    public EduTask<T> Enqueue<T>(Func<T> func)
    {
        var threadItem = new EduThreadPoolItem<T>(func);

        _tasks.Add(threadItem);

        Logger.Log($"Enqueued ID:{threadItem.Task.ID}");

        return threadItem.Task;
    }

    public EduTask Enqueue(Action action)
    {
        throw new NotImplementedException();
    }

    private void StartWork()
    {
        Logger.Log($"Started worker..");

        while(true)
        {
            if (!_tasks.TryTake(out var task))
            {
                Thread.Sleep(1000);
                continue;
            }

            Logger.Log($"Took task ID:{task.ID}");
            task.Invoke();
            Logger.Log($"Invoked action...");
        }
    }
}
