namespace AsyncEduMockUp.Core;

public interface IEduThreadPoolItem<T> : IEduThreadPoolItem;

public interface IEduThreadPoolItem
{
    int ID { get; }
    void Invoke();
}

internal class EduThreadPoolItem(Action action) : IEduThreadPoolItem
{
    public EduTask Task { get; } = new EduTask();

    public int ID => Task.ID;

    public void Invoke()
    {
        action();

        Task.SetResult();
    }
}

internal class EduThreadPoolItem<T>(Func<T> func) : IEduThreadPoolItem<T>
{
    public EduTask<T> Task { get; } = new EduTask<T>();

    public int ID => Task.ID;

    public void Invoke()
    {
        var value = func();

        Task.SetResult(value);
    }
}
