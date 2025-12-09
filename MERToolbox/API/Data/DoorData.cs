using Interactables.Interobjects.DoorUtils;
using MERToolbox.API.Enums;
using MERToolbox.API.Helpers;
using ProjectMER.Features.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace MERToolbox.API.Data
{
    public class DoorData
    {
        public string FileName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public DoorType DoorType { get; set; } = DoorType.EntranceDoor;
        public DoorPermissionFlags Permissions { get; set; } = DoorPermissionFlags.None;
        public bool IsOpen { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public bool RequireAllPermissions { get; set; } = false;
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        internal GameObject GameObject { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        
        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            if (properties.TryGetValue("DoorType", out object doorTypeObj))
            {
                if (doorTypeObj is JsonElement doorTypeElement)
                    this.DoorType = (DoorType)doorTypeElement.GetInt32();
                else
                    this.DoorType = (DoorType)Convert.ToInt32(doorTypeObj);
                
                LogManager.Debug($"Deserialized DoorType: {this.DoorType}");
            }
            else
                return false;

            if (properties.TryGetValue("DoorPermissionFlags", out object permissionsObj))
            {
                if (permissionsObj is JsonElement permissionsElement)
                    this.Permissions = (DoorPermissionFlags)permissionsElement.GetInt32();
                else
                    this.Permissions = (DoorPermissionFlags)Convert.ToInt32(permissionsObj);
                
                LogManager.Debug($"Deserialized DoorPermissionFlags: {this.Permissions}");
            }
            else
                return false;

            if (properties.TryGetValue("IsOpen", out object isOpenObj))
            {
                if (isOpenObj is JsonElement isOpenElement)
                    this.IsOpen = isOpenElement.GetBoolean();
                else
                    this.IsOpen = Convert.ToBoolean(isOpenObj);
                
                LogManager.Debug($"Deserialized IsOpen: {this.IsOpen}");
            }
            else
                return false;

            if (properties.TryGetValue("IsLocked", out object isLockedObj))
            {
                if (isLockedObj is JsonElement isLockedElement)
                    this.IsLocked = isLockedElement.GetBoolean();
                else
                    this.IsLocked = Convert.ToBoolean(isLockedObj);
                
                LogManager.Debug($"Deserialized IsLocked: {this.IsLocked}");
            }
            else
                return false;

            if (properties.TryGetValue("RequireAllPermissions", out object requireAllObj))
            {
                if (requireAllObj is JsonElement requireAllElement)
                    this.RequireAllPermissions = requireAllElement.GetBoolean();
                else
                    this.RequireAllPermissions = Convert.ToBoolean(requireAllObj);
                
                LogManager.Debug($"Deserialized RequireAllPermissions: {this.RequireAllPermissions}");
            }
            else
                return false;

            return true;
        }
    }

    [Serializable]
    public class SerializedDoorData
    {
        public string Name { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public BlockTypes BlockType { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}