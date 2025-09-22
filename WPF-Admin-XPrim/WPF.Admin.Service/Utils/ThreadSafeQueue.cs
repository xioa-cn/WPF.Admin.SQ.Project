using System.Collections.Concurrent;

namespace WPF.Admin.Service.Utils;

/// <summary>
/// FIFO（First In First Out）队列
/// 使用原子操作 防止多线程计数异常
/// </summary>
/// <typeparam name="T"></typeparam>
public class ThreadSafeQueue<T> {
    private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
    private long _count = 0;

    public void Enqueue(T item) {
        _queue.Enqueue(item);
        Interlocked.Increment(ref _count);
    }

    public bool TryDequeue(out T item) {
        if (_queue.TryDequeue(out item))
        {
            Interlocked.Decrement(ref _count);
            return true;
        }

        return false;
    }

    public long Count => Interlocked.Read(ref _count);
}