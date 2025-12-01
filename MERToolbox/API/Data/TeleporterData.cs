using System.Collections.Generic;
using PlayerRoles;

namespace MERToolbox.API.Data
{
    public class TeleporterData
    {
        public string PrimitiveName { get; set; }
        public string TargetPrimitive { get; set; }
        public float CoolDown { get; set; }
        public List<RoleTypeId> AllowedRoles { get; set; }
        public bool PerPlayerCooldown { get; set; }
    }
}