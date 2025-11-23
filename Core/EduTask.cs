using AsyncEduMockUp.Utils;
using System.Diagnostics.CodeAnalysis;

namespace AsyncEduMockUp.Core;

internal class EduTask<T>() : EduTaskBase, IEduTask<T>
{
    private Action<T>? _continuation;
    private T? _result;

    [MaybeNull]
    public T Result => GetResult();

    public EduAwaiter<T> GetAwaiter() => new(this);
    
    public EduTask<T> ContinueWith(Action<T> action)
    {
        Logger.LogDebug($"Asigned continuation... {ID}");

        var task = new EduTask<T>();

        SetContinuation((result) =>
        {
            Logger.LogDebug($"Running continuation... {ID}");

            action(result);

            task.SetResult(result);
        });

        return task;
    }

    public override EduTask ContinueWith(Action action)
    {
        Logger.LogDebug($"Asigned continuation... {ID}");

        var task = new EduTask();

        SetContinuation(_ =>
        {
            Logger.LogDebug($"Running continuation... {ID}");

            action();

            task.SetResult();
        });

        return task;
    }

    public void SetResult(T value)
    {
        Logger.LogDebug($"{value}");

        using var scope = _sync.EnterScope();

        SetResult(scope);

        _result = value;

        Logger.LogDebug($"HasContinuation: {_continuation is { } } {ID}");

        if (_continuation is { } continuation)
        {
            continuation(_result);
        }
    }

    private T GetResult()
    {
        using var scope = _sync.EnterScope();

        EnsureCompleted();

        return _result;
    }

    [MemberNotNull(nameof(_result))]
    private void EnsureCompleted()
    {
        if (!_isCompleted || _result is null)
        {
            throw new InvalidOperationException($"Result is not yet complete.");
        }
    }

    private void SetContinuation(Action<T> continuation)
    {
        using var scope = _sync.EnterScope();

        if (_continuation is not null)
        {
            throw new InvalidOperationException($"Continuation is already set.");
        }

        Logger.LogDebug($"SetContinuation {ID}");

        _continuation = continuation;

        if (_isCompleted)
        {
            EnsureCompleted();

            _continuation(_result);
        }      
    }
}

internal class EduTask() : EduTaskBase, IEduTask
{
    private Action? _continuation;

    public static EduTask WhenAll(IEnumerable<EduTaskBase> tasks) => WhenAll([.. tasks]);
    public static EduTask WhenAll(params EduTaskBase[] tasks)
    {
        var task = new EduTask();
        var count = tasks.Length;

        foreach(var paramTask in tasks)
        {
            paramTask.ContinueWith(() =>
            {
                var newCount = Interlocked.Decrement(ref count);

                if (newCount is 0)
                {
                    task.SetResult();
                }
            });
        }

        return task;
    }

    public static EduTask Run(Action action) => EduThreadPool.Default.Enqueue(action);

    public static EduTask<T> Run<T>(Func<T> func) => EduThreadPool.Default.Enqueue(func);

    public EduAwaiter GetAwaiter() => new(this);

    public override EduTask ContinueWith(Action action)
    {
        Logger.LogDebug($"Asigned continuation... {ID}");

        var task = new EduTask();

        SetContinuation(() =>
        {
            Logger.LogDebug($"Running continuation... {ID}");

            action();

            task.SetResult();
        });

        return task;
    }
    private void SetContinuation(Action continuation)
    {
        using var scope = _sync.EnterScope();

        if (_continuation is not null)
        {
            throw new InvalidOperationException($"Continuation is already set.");
        }

        Logger.LogDebug($"SetContinuation {ID}");

        _continuation = continuation;

        if (_isCompleted)
        {
            EnsureCompleted();

            _continuation();
        }
    }

    private void EnsureCompleted()
    {
        if (!_isCompleted)
        {
            throw new InvalidOperationException($"Result is not yet complete.");
        }
    }

    public void SetResult()
    {
        using var scope = _sync.EnterScope();

        SetResult(scope);

        Logger.LogDebug($"HasContinuation: {_continuation is { }} {ID}");

        if (_continuation is { } continuation)
        {
            continuation();
        }
    }
}
