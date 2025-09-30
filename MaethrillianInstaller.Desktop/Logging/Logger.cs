using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#nullable enable

namespace MaethrillianInstaller.Desktop.Logging
{
    public enum LogLevel
    {
        Debug,
        Information,
        Error
    }

    public sealed class Logger
    {
        private readonly List<LogEntry> entries = new();

        public IReadOnlyList<LogEntry> Entries => entries;

        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            entries.Add(new LogEntry(DateTimeOffset.Now, level, message, exception));
        }

        public bool HasErrors => entries.Exists(entry => entry.Level == LogLevel.Error);

        public void WriteToFile(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? Directory.GetCurrentDirectory());
            File.WriteAllText(path, BuildLogContent(), Encoding.UTF8);
        }

        private string BuildLogContent()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Log generated at {DateTimeOffset.Now:u}");
            builder.AppendLine();

            foreach (var entry in entries)
            {
                builder.AppendLine(entry.ToString());
                if (entry.Exception != null)
                {
                    builder.AppendLine(entry.Exception.ToString());
                }
            }

            return builder.ToString();
        }

        public readonly struct LogEntry
        {
            public LogEntry(DateTimeOffset timestamp, LogLevel level, string message, Exception? exception)
            {
                Timestamp = timestamp;
                Level = level;
                Message = message;
                Exception = exception;
            }

            public DateTimeOffset Timestamp { get; }
            public LogLevel Level { get; }
            public string Message { get; }
            public Exception? Exception { get; }

            public override string ToString()
            {
                return $"[{Timestamp:u}] [{Level}] {Message}";
            }
        }
    }
}
