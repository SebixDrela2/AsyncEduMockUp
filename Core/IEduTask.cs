namespace AsyncEduMockUp.Core;

public interface IEduTask<T> : IEduTask
{
    T? Result { get; }
}

public interface IEduTask
{
    bool IsCompleted { get; }
}
