using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage;

namespace ScreenSoundSwitch.WinUI.Data
{
    public static class DebugLogStore
    {
        private static readonly List<string> _logs = new();
        private static readonly object _syncRoot = new();
        private const long MaxLogFileBytes = 1024 * 1024;
        private static readonly string _sessionName = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        private static readonly string _logDirectory;
        private static int _logFileIndex = 1;
        private static string _currentLogFilePath;

        public static event Action<string>? LogAdded;
        public static event Action? LogsCleared;

        public static IReadOnlyList<string> Logs => _logs;
        public static string LogDirectory => _logDirectory;
        public static string CurrentLogFilePath => _currentLogFilePath;

        static DebugLogStore()
        {
            _logDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logs");
            Directory.CreateDirectory(_logDirectory);
            _currentLogFilePath = CreateLogFilePath();
        }

        public static void Initialize()
        {
            Add($"Log session started. Directory={_logDirectory}");
        }

        public static void Add(string message)
        {
            var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
            lock (_syncRoot)
            {
                _logs.Add(log);
                WriteToFile(log);
            }

            LogAdded?.Invoke(log);
        }

        public static void Clear()
        {
            lock (_syncRoot)
            {
                _logs.Clear();
            }

            LogsCleared?.Invoke();
        }

        private static void WriteToFile(string log)
        {
            try
            {
                var line = log + Environment.NewLine;
                var byteCount = Encoding.UTF8.GetByteCount(line);
                RotateLogFileIfNeeded(byteCount);
                File.AppendAllText(_currentLogFilePath, line, Encoding.UTF8);
            }
            catch
            {
                // 日志写入失败不应影响主功能运行。
            }
        }

        private static void RotateLogFileIfNeeded(long incomingBytes)
        {
            var currentLength = File.Exists(_currentLogFilePath)
                ? new FileInfo(_currentLogFilePath).Length
                : 0;

            if (currentLength + incomingBytes <= MaxLogFileBytes)
            {
                return;
            }

            _logFileIndex++;
            _currentLogFilePath = CreateLogFilePath();
        }

        private static string CreateLogFilePath()
        {
            return Path.Combine(_logDirectory, $"ScreenSoundSwitch-{_sessionName}-{_logFileIndex:000}.log");
        }
    }
}
