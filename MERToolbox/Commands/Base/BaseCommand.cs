using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using MERToolbox.Commands.Admin;
using ProjectMER.Commands.Modifying.Position.SubCommands;
using ProjectMER.Commands.Utility;
using ProjectMER.Features.ToolGun;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERToolbox.Commands.Base
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class BaseCommand : ParentCommand
    {
        public BaseCommand() => LoadGeneratedCommands();

        public override string Command => "mert";

        public override string Description => "Manage the UncomplicatedCustomItems plugin features";

        public override string[] Aliases => [];

        public override void LoadGeneratedCommands()
        {
            Subcommands.Add(new Reload());
            Subcommands.Add(new PrefabTest());
            Subcommands.Add(new SaveLogs());
        }

        internal static List<SubCommandBase> Subcommands { get; } = [];

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = $"MERToolbox v{Plugin.Instance.Version} by Mr. Baguetter\n\n<size=35>Available commands:</size>";
                foreach (SubCommandBase command in Subcommands)
                    response += $"\n- mert {command.Name}{(command.VisibleArgs != string.Empty ? $" {command.VisibleArgs}" : "")} - {command.Description}";

                return true;
            }

            SubCommandBase cmd = Subcommands.FirstOrDefault(cmd => cmd.Name == arguments.At(0));
            cmd ??= Subcommands.FirstOrDefault(cmd => cmd.Aliases.Contains(arguments.At(0)));

            if (cmd is null)
            {
                response = "Command not found!";
                return false;
            }

            if (!sender.HasPermissions(cmd.RequiredPermission))
            {
                response = $"You don't have permission to access that command! \n Required permission: {cmd.RequiredPermission}";
                return false;
            }

            if (arguments.Count < cmd.RequiredArgsCount)
            {
                response = $"Wrong usage!\nCorrect usage: mert {cmd.Name} {cmd.VisibleArgs}";
                return false;
            }

            List<string> args = [.. arguments];
            args.RemoveAt(0);

            return cmd.Execute(args, sender, out response);
        }
    }
}
