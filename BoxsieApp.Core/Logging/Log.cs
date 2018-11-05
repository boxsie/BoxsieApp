using System;
using System.IO;
using System.Threading.Tasks;
using BoxsieApp.Core.Config;
using BoxsieApp.Core;

namespace BoxsieApp.Core.Logging
{
    public enum LogLvl
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public interface ILog
    {
        Task WriteLineAsync(string message, LogLvl lvl = LogLvl.Debug);
        void WriteLine(string message, LogLvl lvl = LogLvl.Debug);
    }

    public class Log : ILog
    {
        private readonly GeneralConfig _config;

        public Log(GeneralConfig config)
        {
            _config = config;

            if (!Directory.Exists(_config.UserConfig.LogOutputPath))
                Directory.CreateDirectory(_config.UserConfig.LogOutputPath);
        }

        public async Task WriteLineAsync(string message, LogLvl lvl = LogLvl.Debug)
        {
            var logMsg = CreateMessage(message, lvl);

            OutputToConsole(logMsg);

            await SaveToDiskAsync(GetLogfilename(), logMsg);
        }

        public void WriteLine(string message, LogLvl lvl = LogLvl.Debug)
        {
            var logMsg = CreateMessage(message, lvl);

            OutputToConsole(logMsg);

            SaveToDisk(GetLogfilename(), logMsg);
        }

        private string GetLogfilename()
        {
            return Path.Combine(_config.UserConfig.LogOutputPath, $"{DateTime.Now:yy-MM-dd}.log");
        }

        private static string CreateMessage(string message, LogLvl lvl = LogLvl.Debug)
        {
            return $"[{DateTime.Now:T}][{lvl.ToString()[0]}]:{message}";
        }

        private static void OutputToConsole(string logMsg)
        {
            Console.WriteLine(logMsg);
        }

        private static void SaveToDisk(string fileName, string logMsg)
        {
            using (var sw = File.AppendText(fileName))
                sw.WriteLine(logMsg);
        }

        private static async Task SaveToDiskAsync(string fileName, string logMsg)
        {
            using (var sw = File.AppendText(fileName))
                await sw.WriteLineAsync(logMsg);
        }
    }
}
