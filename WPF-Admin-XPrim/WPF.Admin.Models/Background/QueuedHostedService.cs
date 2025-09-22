using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using WPF.Admin.Models.Background.Services;

namespace WPF.Admin.Models.Background
{
    public class QueuedHostedService : BackgroundService
    {
        [Required]
        public static IBackgroundTaskQueue TaskQueue { get; }

        static QueuedHostedService()
        {
            TaskQueue = new BackgroundTaskQueue();
        }
        
        public QueuedHostedService()
        {
           
        }

        // 后台执行逻辑（由框架自动调用）
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 循环取任务（直到程序停止，stoppingToken 被触发）
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 从队列获取任务（无任务时阻塞，可响应停止信号）
                    var workItem = await TaskQueue.DequeueAsync(stoppingToken);
                    if (workItem is not null)
                        // 执行任务（传入停止信号，支持任务取消）
                        await workItem(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // 程序停止时触发的正常取消，无需日志
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}