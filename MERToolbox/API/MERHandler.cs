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

        private static void OnSchematicSpawned(SchematicSpawnedEventArgs ev)
        {
            if (ev.Schematic == null || ev.Schematic.AttachedBlocks.IsEmpty())
                return;

            DoorSpawner.SpawnDoor(ev.Schematic);

            if (Plugin.Instance.AudioApi.SoundLists.Count >= 1)
                Plugin.Instance.AudioApi.PlayAudio(ev.Schematic);

            ClutterManager.GenerateClutter(ev.Schematic, out List<SchematicObject> spawnedClutter);
            spawnedClutter.ForEach(c => Logger.Debug($"Generated {c.Name} Clutter item to {ev.Schematic.Name} num: {ClutterManager.ActiveClutter[ev.Schematic].Count()}"));

            foreach (GameObject spawnedObject in ev.Schematic.AttachedBlocks)
            {
                if (!Plugin.Instance.Config.TankData.IsEmpty())
                {
                    foreach (TankData tankData in Plugin.Instance.Config.TankData)
                    {
                        if (spawnedObject.name != tankData.PrimitiveName)
                            continue;

                        Logger.Debug($"Added FluidTank to {spawnedObject.name} - {ev.Schematic.name}");
                        spawnedObject.AddComponent<FluidTank>().Init(LabApi.Features.Wrappers.PrimitiveObjectToy.Get(spawnedObject.GetComponent<AdminToys.PrimitiveObjectToy>()), tankData);
                    }
                }

                if (!Plugin.Instance.Config.KillAreas.IsEmpty() && spawnedObject.TryGetComponent<PrimitiveObjectToy>(out var primitive))
                {
                    foreach (KillArea killArea in Plugin.Instance.Config.KillAreas)
                    {
                        if (spawnedObject.name != killArea.PrimitiveName)
                            continue;

                        LabApi.Features.Wrappers.PrimitiveObjectToy.Get(primitive).Flags = PrimitiveFlags.None;
                        Logger.Debug($"Added KillArea to {spawnedObject.name} - {ev.Schematic.name}");
                        spawnedObject.AddComponent<KillAreaDetector>().Init(spawnedObject);
                    }
                }

                if (!Plugin.Instance.Config.TeleporterData.IsEmpty())
                {
                    foreach (TeleporterData teleporter in Plugin.Instance.Config.TeleporterData)
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