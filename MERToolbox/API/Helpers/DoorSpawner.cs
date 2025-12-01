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
            foreach (DoorData doorData in Plugin.Instance.Config.DoorData)
            {
                foreach (GameObject block in schematic.AttachedBlocks.ToArray())
                {
                    if (block is null)
                        continue;
                    if (block.name != doorData.PrimitiveName)
                        continue;

                    SerializableDoor serializableDoor = new()
                    {
                        DoorType = doorData.DoorType,
                        RequiredPermissions = doorData.Permissions,
                        IsOpen = doorData.IsOpen,
                        IsLocked = doorData.IsLocked,
                        RequireAll = doorData.RequireAllPermissions,
                        Position = block.transform.position,
                        Rotation = block.transform.rotation.eulerAngles,
                        Index = 1
                    };

                    GameObject obj = serializableDoor.SpawnOrUpdateObject();
                    DoorIDs.Add(obj, schematic);
                    schematic._attachedBlocks.Add(obj);
                    NetworkServer.Destroy(block);
                }
            }
        }
    }
}