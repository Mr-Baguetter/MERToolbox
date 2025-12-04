using System;
using System.Collections.Generic;
using System.IO;
using CommandSystem;
using MERToolbox.API.Helpers;
using MERToolbox.Commands.Base;
using static MERToolbox.API.Helpers.LogManager;

namespace MERToolbox.Commands.Admin
{
    public class SaveLogs : SubCommandBase
    {
        public override string Name => "savelogs";
        public override string Description { get; } = "Saves the logs from MERToolbox onto a file.";
        public override string RequiredPermission { get; } = "mertoolbox.savelogs";

        public override bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                string logsDir = Path.Combine(ConfigManager.Dir, "Logs");
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);

                string fileName = $"Log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
                string fullPath = Path.Combine(logsDir, fileName);

                using (StreamWriter logFile = new(fullPath))
                {
                    foreach (Log log in LogManager.logs)
                        logFile.WriteLine($"[{log.TimeStamp:yyyy-MM-dd HH:mm:ss}] {log.Level} {log.Message}");
                }

                response = $"Logs saved to {fullPath}";
                return true;
            }
            catch (Exception ex)
            {
                response = $"Failed to save logs: {ex.Message}";
                return false;
            }
        }
    }
}