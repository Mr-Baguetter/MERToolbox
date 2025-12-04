using System.Collections.Generic;
using PlayerRoles;

namespace MERToolbox.API.Data
{
    public class TeleporterData
    {
        public string PrimitiveName { get; set; } = string.Empty;
        public string TargetPrimitive { get; set; } = string.Empty;
        public float CoolDown { get; set; }
        public List<RoleTypeId> AllowedRoles { get; set; }
        public bool PerPlayerCooldown { get; set; }
    }
}