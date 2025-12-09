using Discord;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERToolbox.API.Helpers
{
    internal class LogManager
    {
        internal class Log(string msg, string level, DateTime timeStamp)
        {
            public string Message { get; set; } = msg;
            public string Level { get; set; } = level;
            public DateTime TimeStamp { get; set; } = timeStamp;
        }

        public static List<Log> logs = [];

        public static void Debug(string message)
        {
            if (Plugin.Instance.Config.Debug)
                Logger.Raw($"[DEBUG] [{Plugin.Instance.GetType().Assembly.GetName().Name}] {message}", ConsoleColor.Green);

            logs.Add(new Log(message, "[DEBUG]", DateTime.Now));
        }

        public static void Info(string message)
        {
            Logger.Info(message);
            logs.Add(new Log(message, "[INFO]", DateTime.Now));
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
            logs.Add(new Log(message, "[WARN]", DateTime.Now));
        }

        public static void Error(string message)
        {
            Logger.Error(message);
            logs.Add(new Log(message, "[ERROR]", DateTime.Now));
        }

        public static void Raw(string message, string category, ConsoleColor color)
        {
            Logger.Raw($"[{category}] [{Plugin.Instance.GetType().Assembly.GetName().Name}] {message}", color);
            logs.Add(new Log(message, $"[{category}]", DateTime.Now));
        }
    }
}
