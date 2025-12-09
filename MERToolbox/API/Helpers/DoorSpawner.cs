using Mirror;
using ProjectMER.Features.Objects;
using ProjectMER.Features.Serializable;
using MERToolbox.API.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MERToolbox.API.Helpers
{
    public class DoorSpawner
    {
        public static Dictionary<GameObject, SchematicObject> DoorIDs { get; set; } = [];

        public static void SpawnDoor(SchematicObject schematic)
        {
            foreach (DoorData doorData in ConfigManager.DoorData.Where(d => d.FileName == schematic.Name))
            {
                SerializableDoor serializableDoor = new()
                {
                    DoorType = doorData.DoorType,
                    RequiredPermissions = doorData.Permissions,
                    IsOpen = doorData.IsOpen,
                    IsLocked = doorData.IsLocked,
                    RequireAll = doorData.RequireAllPermissions,
                    Position = Vector3.zero,
                    Rotation = Vector3.zero,
                    Index = 1
                };

                GameObject obj = serializableDoor.SpawnOrUpdateObject();
                doorData.GameObject = obj;
                NetworkServer.UnSpawn(obj);
                ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, doorData.Position, doorData.Rotation, out Vector3 position, out Quaternion rotation);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                DoorIDs.Add(obj, schematic);
                NetworkServer.Spawn(obj);
                LogManager.Debug($"Spawning Door at {obj.transform.position} - {schematic.Position} - {obj.transform.localPosition}");
                schematic._attachedBlocks.Add(obj);
            }
        }
    }
}