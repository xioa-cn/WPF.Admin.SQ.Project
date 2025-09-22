namespace WPF.Admin.Models.Background.Services
{
    public interface IBackgroundTaskQueue
    {
        // 提交任务到队列
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    
        // 从队列获取任务（异步等待，直到有任务可用）
        Task<Func<CancellationToken, Task>?> DequeueAsync(CancellationToken cancellationToken);
    }
}