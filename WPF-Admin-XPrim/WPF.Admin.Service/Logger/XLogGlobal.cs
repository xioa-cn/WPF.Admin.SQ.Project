using WPF.Admin.Models.Db;
using WPF.Admin.Models.Models;
using WPF.Admin.Models.Utils;
using WPF.Xlog.Logger.Impl;
using WPF.Xlog.Logger.Model;
using WPF.Xlog.Logger.Service;

namespace WPF.Admin.Service.Logger {
    public class XLogGlobal {
        private static int logLock = 0;
        private static int logDay = -1;

        public static ILogService? Logger {
            get
            {
               

                if (logLock == 0)
                {
                    Interlocked.Exchange(ref logLock, 1);
                    logDay = DateTime.Now.Day;
                    Task.Run(ClearLogger);
                }

                if (logDay != DateTime.Now.Day)
                {
                    Interlocked.Exchange(ref logLock, 0);
                }

                return LogService.Instance;
            }
        }

        private static string? _logDirectory;

        private static string logDirectory {
            get { return _logDirectory ??= System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XLogs"); }
        }

        private static double _logTime => AppSettings.Default?.logDuration ?? 15.0;

        private static double _logDuration {
            get { return _logTime; }
        }

        static XLogGlobal() {
            LogService.CreateLoggerInstance(logDirectory, maxFileSizeInMB: 1000, maxLogFiles: 10);
            if (AppSettings.Default is null || !AppSettings.Default.OpenDblog)
            {
                return;
            }

            var logdbPath = System.IO.Path.Combine(logDirectory, "DBLOGGER");
            if (!System.IO.Directory.Exists(logdbPath))
            {
                System.IO.Directory.CreateDirectory(logdbPath);
            }

            logdbPath = System.IO.Path.Combine(logDirectory, "DBLOGGER", "XLog.db");
            try
            {
                using (var _logDbContext = new LogDbContext(logdbPath))
                {
                    if (!System.IO.File.Exists(logdbPath))
                    {
                        _logDbContext.Database.EnsureCreated();
                    }


                    var firstlog = _logDbContext.DbLogEntries.FirstOrDefault();
                    if (firstlog is not null)
                    {
                        if (firstlog.Timestamp < DateTime.Now.AddDays(-_logDuration))
                        {
                            _logDbContext.DbLogEntries.RemoveRange(
                                _logDbContext.DbLogEntries.Where(e =>
                                    e.Timestamp < DateTime.Now.AddDays(-_logDuration)));
                        }
                    }

                    var alarmDbContext = AlarmDbInstance.CreateNormal();
                    var firstAlarm = alarmDbContext.AlarmLogs.FirstOrDefault();
                    if (firstAlarm is not null)
                    {
                        if (firstAlarm.CreateTime < DateTime.Now.AddDays(-_logDuration))
                        {
                            alarmDbContext.AlarmLogs.RemoveRange(
                                alarmDbContext.AlarmLogs.Where(e =>
                                    e.CreateTime < DateTime.Now.AddDays(-_logDuration)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorLog = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 日志模块初始化失败: {ex.Message}\n";
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(logDirectory, "error.log"), errorLog);
            }


            LogService.Instance?.AddLogAction(entry =>
            {
                try
                {
                    using var _logDbContext = new LogDbContext(logdbPath);
                    _logDbContext.DbLogEntries.Add(new DbLogEntry {
                        Timestamp = entry.Timestamp,
                        Level = entry.Level,
                        Message = entry.Message,
                        Source = entry.Source,
                        UserName = entry.UserName,
                        Exception = entry.Exception?.Message,
                        AdditionalInfo = entry.AdditionalInfo
                    });
                    _logDbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    var errorLog = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 日志记录失败: {ex.Message}\n";
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(logDirectory, "error.log"), errorLog);
                }
            });
        }

        private static void ClearLogger() {
            if (!System.IO.Directory.Exists(logDirectory))
            {
                return;
            }

            var directoryInfo = new System.IO.DirectoryInfo(logDirectory);
            var files = directoryInfo.GetDirectories();
            foreach (var item in files)
            {
                var dirName = System.IO.Path.GetFileNameWithoutExtension(item.FullName);
                if (!DateTime.TryParse(dirName, out var time))
                {
                    continue;
                }

                if (time.AddDays(_logDuration) >= DateTime.Now)
                {
                    continue;
                }

                try
                {
                    item.Delete(true);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}