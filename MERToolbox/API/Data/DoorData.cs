using Interactables.Interobjects.DoorUtils;
using ProjectMER.Features.Enums;

namespace MERToolbox.API.Data
{
    public class DoorData
    {
        public string PrimitiveName { get; set; } = string.Empty;
        public DoorType DoorType { get; set; } = DoorType.EntranceDoor;
        public DoorPermissionFlags Permissions { get; set; } = DoorPermissionFlags.None;
        public bool IsOpen { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool RequireAllPermissions { get; set; } = false;
    }
}
