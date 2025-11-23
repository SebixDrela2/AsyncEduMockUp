using AsyncEduMockUp.Utils;
using System.Collections.Concurrent;

namespace AsyncEduMockUp.Core;

internal class EduThreadPool(int capacity) : IDisposable
{
    private int _taskCount = 0;
    private ConcurrentBag<Thread> _workers = [];

    private readonly ManualResetEventSlim _resetEvent = new(true);
    private readonly ConcurrentBag<IEduThreadPoolItem> _tasks = [];

    private static readonly EduThreadPool _default = Create(Environment.ProcessorCount);
    public static EduThreadPool Default => _default;

    public static EduThreadPool Create(int threadCount) 
        => new(threadCount);

    public EduTask<T> Enqueue<T>(Func<T> func)
    {
        var threadItem = new EduThreadPoolItem<T>(func);
        AddTask(threadItem);

        Logger.LogDebug($"Enqueued ID:{threadItem.Task.ID}");

        return threadItem.Task;
    }

    public EduTask Enqueue(Action action)
    {
        var threadItem = new EduThreadPoolItem(action);

        AddTask(threadItem);

        Logger.LogDebug($"Enqueued ID:{threadItem.Task.ID}");

        return threadItem.Task;
    }

    private void AddTask(IEduThreadPoolItem threadItem)
    {
        var newCount = Interlocked.Increment(ref _taskCount);

        if (newCount is 1)
        {
            _resetEvent.Reset();
        }

        if (newCount < capacity)
        {
            StartWorker();

            Logger.LogInfo($"Start worker...");
        }
        else
        {
            newCount = Interlocked.Decrement(ref _taskCount);
        }

        _tasks.Add(threadItem);
    }

    private void StartWorker()
    {
        var worker = new Thread(StartWork);
        worker.Start();

        _workers.Add(worker);
    }

    public void Dispose()
    {
        _resetEvent.Wait();
        _resetEvent.Dispose();
    }

    private void StartWork()
    {
        try
        {
            Logger.LogDebug($"Started worker..");

            while (true)
            {
                if (!_tasks.TryTake(out var task))
                {
                    break;
                }

                Logger.LogDebug($"Took task ID:{task.ID}");

                task.Invoke();

                Logger.LogDebug($"Invoked action...");
            }
        }
        finally
        {
            var newCount = Interlocked.Decrement(ref _taskCount);

            if (newCount is 0)
            {
                _resetEvent.Set();
            }
        }
    }
}
