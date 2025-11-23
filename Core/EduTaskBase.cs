using System.Diagnostics.CodeAnalysis;

namespace AsyncEduMockUp.Core;

internal abstract class EduTaskBase
{
    private static int _nextId = 1;

    protected readonly Lock _sync = new();
    private protected bool _isCompleted;

    public int ID = Interlocked.Increment(ref _nextId);

    public bool IsCompleted 
    {
        get => Volatile.Read(ref _isCompleted);
        set => Volatile.Write(ref _isCompleted, value);
    }

    private protected void SetResult(Lock.Scope scope)
    {
        if (_isCompleted)
        {
            throw new InvalidOperationException($"Result is already completed.");
        }

        _isCompleted = true;
    }
}
