using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERToolbox.Commands.Base
{
    public abstract class SubCommandBase
    {
        public abstract string Name { get; }

        public virtual string VisibleArgs { get; }

        public virtual int RequiredArgsCount { get; }

        public abstract string Description { get; }

        public virtual string[] Aliases { get; }

        public virtual string RequiredPermission { get; }

        public abstract bool Execute(List<string> arguments, ICommandSender sender, out string response);
    }
}
