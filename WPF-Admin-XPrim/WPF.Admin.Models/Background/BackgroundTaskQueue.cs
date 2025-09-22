using System.Collections.Concurrent;
using WPF.Admin.Models.Background.Services;

namespace WPF.Admin.Models.Background
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        // 存储后台任务的并发队列
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
    
        // 控制并发的信号量（初始0，有任务时释放信号）
        private readonly SemaphoreSlim _signal = new(0);

        // 提交任务
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null) throw new ArgumentNullException(nameof(workItem));
        
            _workItems.Enqueue(workItem);
            _signal.Release(); // 释放信号，通知工作者有任务可用
        }

        // 异步获取任务（无任务时阻塞，直到有任务或取消）
        public async Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken); // 等待信号（可响应取消）
            _workItems.TryDequeue(out var workItem); // 从队列取出任务
        
            return workItem;
        }
    }
}