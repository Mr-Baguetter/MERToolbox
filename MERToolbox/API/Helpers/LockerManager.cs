using System;
using System.Collections.Generic;
using MERToolbox.API.Data;
using ProjectMER.Features.Objects;
using UnityEngine;
using static MERToolbox.API.Data.LockerData;
using LockerChamber = MapGeneration.Distributors.LockerChamber;
using Locker = LabApi.Features.Wrappers.Locker;
using Mirror;
using MapGeneration.Distributors;

namespace MERToolbox.API.Helpers
{
    public class LockerManager
    {
        public static Dictionary<SchematicObject, List<LockerData>> LockersBySchematic = [];

        public static GameObject GetLockerPrefab(MERTLockerType type)
        {
            return type switch
            {
                MERTLockerType.Adrenaline => ProjectMER.Features.PrefabManager.LockerAdrenalineMedkit.gameObject,
                MERTLockerType.Pedestal => ProjectMER.Features.PrefabManager.PedestalScp500.gameObject,
                MERTLockerType.LargeGun => ProjectMER.Features.PrefabManager.LockerLargeGun.gameObject,
                MERTLockerType.Medkit => ProjectMER.Features.PrefabManager.LockerRegularMedkit.gameObject,
                MERTLockerType.Misc => ProjectMER.Features.PrefabManager.LockerMisc.gameObject,
                MERTLockerType.RifleRack => ProjectMER.Features.PrefabManager.LockerRifleRack.gameObject,
                _ => throw new InvalidOperationException(),
            };
        }

        public static void SpawnLockers(SchematicObject schematic)
        {
            if (!LockersBySchematic.ContainsKey(schematic))
                LockersBySchematic[schematic] = [];

            List<LockerChamber> Spawned = [];
            foreach(LockerData lockerData in ConfigManager.Lockers.ToArray())
            {
                if (lockerData.FileName == schematic.Name)
                {
                    if (UnityEngine.Random.Range(0, 101) > lockerData.Chance)
                        continue;

                    GameObject lockerPrefab = UnityEngine.Object.Instantiate(GetLockerPrefab(lockerData.LockerType));
                    ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, lockerData.Position, lockerData.Rotation, out Vector3 position, out Quaternion rotation);
                    lockerData.GameObject = lockerPrefab;
                    lockerPrefab.transform.position = position;
                    lockerPrefab.transform.rotation = rotation;
                    LockersBySchematic[schematic].Add(lockerData);
                    if (lockerPrefab.TryGetComponent<StructurePositionSync>(out var posSync))
                    {
                        posSync.Network_position = position;
                        posSync.Network_rotationY = (sbyte)Mathf.RoundToInt(rotation.eulerAngles.y / 5.625f);
                    }
                    if (lockerPrefab.TryGetComponent<MapGeneration.Distributors.Locker>(out var component))
                    {
                        component.ParentRoom = RoomExtensions.GetClosestRoomToPosition(position);
                        LogManager.Debug($"{component.ParentRoom.Name}");
                        lockerPrefab.SetActive(true);
                        NetworkServer.Spawn(lockerPrefab);
                        LogManager.Debug($"Spawned locker at {position} - {rotation} - {component.ParentRoom.Name}");
                        Locker locker = Locker.Get(component);
                        locker.ClearAllChambers();
                        locker.ClearLockerLoot();
                        foreach (ChamberData chamberData in lockerData.Chambers)
                        {
                            foreach (LockerChamber chamber in component.Chambers)
                            {
                                if (chamberData.Chamber != component.Chambers.IndexOf(chamber))
                                    continue;
                                
                                chamber.RequiredPermissions = chamberData.Permissions;
                                foreach (ChamberItemData itemData in chamberData.ItemData)
                                {
                                    if (UnityEngine.Random.Range(0, 101) > itemData.Chance)
                                        continue;
                                    if (Spawned.Contains(chamber))
                                        continue;

                                    chamber.SpawnItem(itemData.Item, itemData.Amount);
                                    LogManager.Debug($"Spawned item in Chamber {component.Chambers.IndexOf(chamber)}");
                                    Spawned.Add(chamber);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DestroyLockers(SchematicObject schematic)
        {
            if (!LockersBySchematic.ContainsKey(schematic))
                return;

            foreach (LockerData locker in LockersBySchematic[schematic])
                NetworkServer.Destroy(locker.GameObject);
        }
    }
}