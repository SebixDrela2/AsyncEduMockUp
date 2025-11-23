using AsyncEduMockUp.Utils;
using System.Collections.Concurrent;

namespace AsyncEduMockUp.Core;

internal class EduThreadPool(Thread[] workers) : IDisposable
{
    private Thread[]? _workers = workers;
    private CountdownEvent _countDownEvent = new(workers.Length);
    private CancellationTokenSource _cts = new();

    private readonly ConcurrentBag<IEduThreadPoolItem> _tasks = [];

    private static readonly EduThreadPool _default = Create(Environment.ProcessorCount);
    public static EduThreadPool Default => _default;

    public static EduThreadPool Create(int threadCount)
    {
        var workers = new Thread[threadCount];
        var instance = new EduThreadPool(workers);

        foreach(ref var worker in workers.AsSpan())
        {
            worker = new Thread(instance.StartWork);
            worker.Start();

            Logger.LogInfo($"Start worker...");
        }

        return instance;
    }

    public EduTask<T> Enqueue<T>(Func<T> func)
    {
        var threadItem = new EduThreadPoolItem<T>(func);

        _tasks.Add(threadItem);

        Logger.LogDebug($"Enqueued ID:{threadItem.Task.ID}");

        return threadItem.Task;
    }

    public EduTask Enqueue(Action action)
    {
        var threadItem = new EduThreadPoolItem(action);

        _tasks.Add(threadItem);

        Logger.LogDebug($"Enqueued ID:{threadItem.Task.ID}");

        return threadItem.Task;
    }

    public void Dispose()
    {
        _cts.Cancel();

        _countDownEvent.Wait();

        _cts.Dispose();
        _countDownEvent.Dispose();
    }

    private void StartWork()
    {
        try
        {
            Logger.LogDebug($"Started worker..");
            var token = _cts.Token;

            while (!token.IsCancellationRequested)
            {
                if (!_tasks.TryTake(out var task))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Logger.LogDebug($"Took task ID:{task.ID}");
                task.Invoke();
                Logger.LogDebug($"Invoked action...");
            }
        }
        finally
        {
            _countDownEvent.Signal();
        }
    }
}
