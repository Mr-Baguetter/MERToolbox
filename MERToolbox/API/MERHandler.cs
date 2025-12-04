using AdminToys;
using Mirror;
using ProjectMER.Events.Arguments;
using ProjectMER.Features.Objects;
using MERToolbox.API.Components;
using MERToolbox.API.Data;
using MERToolbox.API.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MEREvent = ProjectMER.Events.Handlers.Schematic;

namespace MERToolbox.API
{
    public class MERHandler
    {
        public static void Register()
        {
            MEREvent.SchematicSpawned += OnSchematicSpawned;
            MEREvent.SchematicDestroyed += OnSchematicDestroyed;
        }

        public static void Unregister()
        {
            MEREvent.SchematicSpawned -= OnSchematicSpawned;
            MEREvent.SchematicDestroyed -= OnSchematicDestroyed;
        }

        public static List<SchematicObject> LoadedSchematicObjects = [];

        private static void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
        {
            if (ev.Schematic == null)
                return;

            LogManager.Debug(ev.Schematic.Name);
            LoadedSchematicObjects.Add(ev.Schematic);
            DoorSpawner.SpawnDoor(ev.Schematic);

            if (Plugin.Instance.AudioApi.SoundLists.Count >= 1)
                Plugin.Instance.AudioApi.PlayAudio(ev.Schematic);

            ClutterManager.GenerateClutter(ev.Schematic, out List<GameObject> spawnedClutter);
            if (Plugin.Instance.Config.Debug)
            {
                foreach (GameObject gameObject in spawnedClutter)
                {
                    LogManager.Debug($"Generated {gameObject.name} Clutter item to {ev.Schematic.Name} num: {ClutterManager.ActiveClutter[ev.Schematic].Count}");
                }
            }

            foreach (GameObject spawnedObject in ev.Schematic.AttachedBlocks)
            {
                if (!ConfigManager.TankData.IsEmpty())
                {
                    foreach (TankData tankData in ConfigManager.TankData)
                    {
                        if (spawnedObject.name != tankData.PrimitiveName)
                            continue;

                        LogManager.Debug($"Added FluidTank to {spawnedObject.name} - {ev.Schematic.name}");
                        spawnedObject.AddComponent<FluidTank>().Init(LabApi.Features.Wrappers.PrimitiveObjectToy.Get(spawnedObject.GetComponent<PrimitiveObjectToy>()), tankData);
                    }
                }

                if (!ConfigManager.KillAreas.IsEmpty() && spawnedObject.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                {
                    foreach (KillArea killArea in ConfigManager.KillAreas)
                    {
                        if (spawnedObject.name != killArea.PrimitiveName)
                            continue;

                        LabApi.Features.Wrappers.PrimitiveObjectToy.Get(primitive).Flags = PrimitiveFlags.None;
                        LogManager.Debug($"Added KillArea to {spawnedObject.name} - {ev.Schematic.name}");
                        spawnedObject.AddComponent<KillAreaDetector>().Init(spawnedObject);
                    }
                }

                if (!ConfigManager.TeleporterData.IsEmpty())
                {
                    foreach (TeleporterData teleporter in ConfigManager.TeleporterData)
                    {
                        if (spawnedObject.name != teleporter.PrimitiveName)
                            continue;

                        GameObject targetObject = ev.Schematic.AttachedBlocks.FirstOrDefault(o => o.name == teleporter.TargetPrimitive);
                        if (targetObject is null)
                            continue;
                        
                        spawnedObject.AddComponent<Teleporter>().Init(teleporter, spawnedObject, targetObject);
                    }
                }
            }
        }

        private static void OnSchematicDestroyed(SchematicDestroyedEventArgs ev)
        {
            LoadedSchematicObjects.Remove(ev.Schematic);
            foreach (var door in DoorSpawner.DoorIDs)
            {
                if (ev.Schematic == door.Value)
                    NetworkServer.Destroy(door.Key);
            }

            if (AudioApi.AudioPlayers.TryGetValue(ev.Schematic, out List<AudioPlayer> audioPlayers))
            {
                foreach (AudioPlayer audioPlayer in audioPlayers)
                {
                    audioPlayer.Destroy();
                }
            }
        }
    }
}