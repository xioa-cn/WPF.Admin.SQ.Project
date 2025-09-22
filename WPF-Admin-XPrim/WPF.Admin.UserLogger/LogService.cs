using System.IO;
using System.Text;

namespace WPF.Admin.UserLogger
{
    public class UserLogService : IUserLogService
    {
        /// <summary>
        /// 单例实例
        /// </summary>
        public static UserLogService? Instance { get; private set; }

        public static void CreateLoggerInstance(string logDirectory = "XUserLogs", int maxFileSizeInMB = 100,
            int maxLogFiles = 5)
        {
            Instance = new UserLogService(logDirectory, maxFileSizeInMB, maxLogFiles);
        }

        /// <summary>
        /// 日志根目录
        /// </summary>
        private readonly string _baseLogDirectory;

        /// <summary>
        /// 用于线程同步的锁对象
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// 单个日志文件的最大大小（MB）
        /// </summary>
        private readonly int _maxFileSizeInMB;

        /// <summary>
        /// 每个级别最多保留的日志文件数
        /// </summary>
        private readonly int _maxLogFiles;

        /// <summary>
        /// 私有构造函数，确保单例模式
        /// </summary>
        /// <param name="logDirectory">日志根目录</param>
        /// <param name="maxFileSizeInMB">单个文件最大大小（MB）</param>
        /// <param name="maxLogFiles">每个级别最多保留的文件数</param>
        private UserLogService(string logDirectory = "XUserLogs", int maxFileSizeInMB = 100, int maxLogFiles = 5)
        {
            _baseLogDirectory = logDirectory;
            _maxFileSizeInMB = maxFileSizeInMB;
            _maxLogFiles = maxLogFiles;

            Directory.CreateDirectory(_baseLogDirectory);
        }

        private Action<LogEntry>? LogAction;

        public void AddLogAction(Action<LogEntry> action)
        {
            LogAction += action;
        }

        /// <summary>
        /// 获取日志文件路径
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="path">日志文件路径</param>
        /// <returns>可用的日志文件路径</returns>
        /// <remarks>
        /// 1. 按日期和级别创建目录
        /// 2. 检查文件大小，必要时创建新文件
        /// 3. 维护文件数量，超出限制时删除最旧的文件
        /// </remarks>
        private string GetLogFilePath(LogLevel level, string path = "")
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            string levelDirectory = string.Empty;

            if (!string.IsNullOrEmpty(path))
            {
                levelDirectory = Path.Combine(_baseLogDirectory, currentDate, path, level.ToString());
            }
            else
            {
                levelDirectory = Path.Combine(_baseLogDirectory, currentDate, level.ToString());
            }

            // 确保目录存在
            if (!Directory.Exists(levelDirectory))
            {
                Directory.CreateDirectory(levelDirectory);
            }

            // 获取当前日志文件列表
            var logFiles = Directory.GetFiles(levelDirectory, "*.log")
                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                .ToList();

            // 如果没有文件，创建一个文件
            if (!logFiles.Any())
            {
                return Path.Combine(levelDirectory, "1.log");
            }

            // 检查当前文件大小
            string currentFile = logFiles.Last();
            if (new FileInfo(currentFile).Length >= _maxFileSizeInMB * 1024 * 1024)
            {
                int nextNumber = logFiles.Count + 1;
                if (nextNumber > _maxLogFiles)
                {
                    // 超出文件数限制，删除最旧的并重命名其他文件
                    File.Delete(logFiles.First());
                    for (int i = 1; i < logFiles.Count; i++)
                    {
                        File.Move(logFiles[i], Path.Combine(levelDirectory, $"{i}.log"));
                    }

                    nextNumber = logFiles.Count;
                }

                return Path.Combine(levelDirectory, $"{nextNumber}.log");
            }

            return currentFile;
        }

        /// <summary>
        /// 写入日志条目
        /// </summary>
        /// <param name="entry">日志条目</param>
        /// <param name="path">日志文件路径，默认为空</param>
        /// <remarks>
        /// 使用锁确保线程安全
        /// 写入失败时尝试写入Windows事件日志
        /// </remarks>
        private void WriteLogEntry(LogEntry entry, string path = "")
        {
            var logMessage = FormatLogEntry(entry);

            lock (_lockObj)
            {
                try
                {
                    string logFilePath = GetLogFilePath(entry.Level, path);
                    File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
                    LogAction?.Invoke(entry);
                }
                catch (Exception ex)
                {
                    try
                    {
                        System.Diagnostics.EventLog.WriteEntry("DataAcquisition",
                            $"Failed to write log: {ex.Message}\nOriginal log: {logMessage}",
                            System.Diagnostics.EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 格式化日志条目
        /// </summary>
        /// <param name="entry">日志条目</param>
        /// <returns>格式化后的日志文本</returns>
        private string FormatLogEntry(LogEntry entry)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{entry.Level}] {entry.Message}");

            if (!string.IsNullOrEmpty(entry.UserName))
                sb.AppendLine($"User: {entry.UserName}");

            if (entry.Exception != null)
            {
                sb.AppendLine($"Exception: {entry.Exception.GetType().Name}");
                sb.AppendLine($"Message: {entry.Exception.Message}");
                sb.AppendLine($"StackTrace: {entry.Exception.StackTrace}");
            }

            sb.AppendLine($"Source:{entry.Source}");
            if (!string.IsNullOrEmpty(entry.AdditionalInfo))
                sb.AppendLine($"Additional Info: {entry.AdditionalInfo}");

            return sb.ToString();
        }

        public void Log(LogLevel level, string message, Exception? exception = null, string? source = "",
            string path = "")
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                var stackTrace = new System.Diagnostics.StackTrace(true);
                var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
                var callerMethod = callerFrame?.GetMethod();
                var callerType = callerMethod?.DeclaringType;
                source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            }

            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                // 格式化调用源信息：类名.方法名 (文件名:行号)
                Source = source,
                UserName = Environment.UserName,
                Exception = exception,
                AdditionalInfo = exception?.StackTrace
            };

            WriteLogEntry(logEntry, path);
        }

        public void LogDebug(string message)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1);
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            Log(LogLevel.Debug, message, null, source);
        }

        public void LogInfo(string message, string path = "")
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1);
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            Log(LogLevel.Info, message, null, source, path);
        }

        public void LogWarning(string message, string path = "")
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1);
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            Log(LogLevel.Warning, message, null, source, path);
        }

        public void LogError(string message, Exception? ex = null, string path = "")
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1); // 跳过当前方法
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            Log(LogLevel.Error, message, ex, source, path);
        }

        public void LogFatal(string message, Exception? ex = null, string path = "")
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1);
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            Log(LogLevel.Fatal, message, ex, source, path);
        }

        public void LogUserAction(string userName, string action, string details, string path = "")
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            var callerFrame = stackTrace.GetFrame(1);
            var callerMethod = callerFrame?.GetMethod();
            var callerType = callerMethod?.DeclaringType;
            var source = $"{callerType?.Name}.{callerMethod?.Name} " +
                         $"({Path.GetFileName(callerFrame?.GetFileName())}:{callerFrame?.GetFileLineNumber()})";
            var message = $"User Action: {action} - {details}";
            Log(LogLevel.UserAction, message, null, source, path);
        }
    }
}