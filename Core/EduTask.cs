using AsyncEduMockUp.Utils;
using System.Diagnostics.CodeAnalysis;

namespace AsyncEduMockUp.Core;

internal class EduTask<T>() : EduTaskBase, IEduTask<T>
{
    private Action<T>? _continuation;
    private T? _result;

    [MaybeNull]
    public T Result => GetResult();

    public EduTask<T> ContinueWith(Action<T> action)
    {
        Logger.Log($"Asigned continuation... {ID}");

        var task = new EduTask<T>();

        SetContinuation((result) =>
        {
            Logger.Log($"Running continuation... {ID}");

            action(result);

            task.SetResult(result);
        });

        return task;
    }

    public void SetResult(T value)
    {
        Logger.Log($"{value}");

        using var scope = _sync.EnterScope();

        SetResult(scope);

        _result = value;

        Logger.Log($"HasContinuation: {_continuation is { } } {ID}");

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

        Logger.Log($"SetContinuation {ID}");

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
    public static EduTask Run(Action action) => EduThreadPool.Default.Enqueue(action);

    public static EduTask<T> Run<T>(Func<T> func) => EduThreadPool.Default.Enqueue(func);

    public void SetResult()
    {
        using var scope = _sync.EnterScope();

        SetResult(scope);
    }
}
