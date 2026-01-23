using System;
using System.Collections.Generic;
using System.Linq;
using AdminToys;
using CommandSystem;
using MERToolbox.API;
using MERToolbox.API.Components;
using MERToolbox.API.Data;
using MERToolbox.API.Helpers;
using MERToolbox.Commands.Base;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace MERToolbox.Commands.Admin
{
    public class Load : SubCommandBase
    {
        public override string Name => "load";
        public override string Description { get; } = "Loads the specified MERT items.";
        public override string RequiredPermission { get; } = "mertoolbox.load";
        public override int RequiredArgsCount => 1;
        public override string VisibleArgs => "Schematic Name";

        public override bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            string schematicName = string.Join(" ", arguments);
            SchematicObject? schematic = MERHandler.LoadedSchematicObjects.FirstOrDefault(s => s.Name.Equals(schematicName, StringComparison.OrdinalIgnoreCase));
            if (schematic is null)
            {
                response = $"Schematic with name '{schematicName}' not found.";
                return false;
            }
            
            DoorSpawner.SpawnDoor(schematic);
            CustomItemManager.TrySpawnItems(schematic);
            CameraManager.SpawnCamera(schematic);
            LockerManager.SpawnLockers(schematic);
            Plugin.Instance.AudioApi.PlayAudio(schematic);

            ClutterManager.GenerateClutter(schematic, out List<GameObject> spawnedClutter);
            if (Plugin.Instance.Config.Debug)
            {
                foreach (GameObject gameObject in spawnedClutter)
                {
                    LogManager.Debug($"Generated {gameObject.name} Clutter item to {schematic.Name} num: {ClutterManager.ActiveClutter[schematic].Count}");
                }
            }

            foreach (GameObject spawnedObject in schematic.AttachedBlocks)
            {
                if (!ConfigManager.TankData.IsEmpty())
                {
                    foreach (TankData tankData in ConfigManager.TankData)
                    {
                        if (spawnedObject.name != tankData.PrimitiveName)
                            continue;

                        LogManager.Debug($"Added FluidTank to {spawnedObject.name} - {schematic.name}");
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
                        LogManager.Debug($"Added KillArea to {spawnedObject.name} - {schematic.name}");
                        spawnedObject.AddComponent<KillAreaDetector>().Init(spawnedObject);
                    }
                }

                if (!ConfigManager.TeleporterData.IsEmpty())
                {
                    foreach (TeleporterData teleporter in ConfigManager.TeleporterData)
                    {
                        if (spawnedObject.name != teleporter.PrimitiveName)
                            continue;

                        GameObject targetObject = schematic.AttachedBlocks.FirstOrDefault(o => o.name == teleporter.TargetPrimitive);
                        if (targetObject is null)
                            continue;
                        
                        spawnedObject.AddComponent<Teleporter>().Init(teleporter, spawnedObject, targetObject);
                    }
                }
            }

            response = $"Loaded {schematicName}";
            return true;
        }
    }
}