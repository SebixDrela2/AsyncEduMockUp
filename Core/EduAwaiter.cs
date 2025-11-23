using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AsyncEduMockUp.Core;

internal class EduAwaiter(EduTask task) : INotifyCompletion, ICriticalNotifyCompletion
{
    public bool IsCompleted => task.IsCompleted;

    public void GetResult()
    {
        task.Wait();
    }

    public void OnCompleted(Action continuation)
    {
        task.ContinueWith(continuation);
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        task.ContinueWith(continuation);
    }
}

internal class EduAwaiter<T>(EduTask<T> task) : INotifyCompletion, ICriticalNotifyCompletion
{
    public bool IsCompleted => task.IsCompleted;

    public T? GetResult()
    {
        task.Wait();

        return task.Result;
    }
    public void OnCompleted(Action continuation)
    {
        task.ContinueWith(_ => continuation());
    }

    public void UnsafeOnCompleted(Action continuation)
    {
        task.ContinueWith(_ => continuation());
    }
}
