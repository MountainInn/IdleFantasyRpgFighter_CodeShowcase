using UniRx;
using System.Collections.Generic;

public class ReactiveQueue<T> : Queue<T>
{
    public ReactiveProperty<T> next = new();

    public new T Dequeue()
    {
        T result = base.Dequeue();

        if (TryPeek(out T f))
            next.Value = f;
        else
            next.Value = default;

        return result;
    }

    public new void Enqueue(T item)
    {
        base.Enqueue(item);

        if (Count == 1)
            next.Value = item;
    }

    public new bool TryDequeue(out T result)
    {
        bool success = base.TryDequeue(out result);

        if (success)
            if (TryPeek(out T f))
                next.Value = f;
            else
                next.Value = default;

        return success;
    }
}
