using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using MERToolbox.API;
using MERToolbox.API.Components;
using MERToolbox.API.Helpers;
using MERToolbox.Commands.Base;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace MERToolbox.Commands.Admin
{
    public class Unload : SubCommandBase
    {
        public override string Name => "unload";
        public override string Description { get; } = "Unloads the specified MERT items.";
        public override string RequiredPermission { get; } = "mertoolbox.unload";
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

            ClutterManager.RemoveClutter(schematic);
            DoorSpawner.DestroyDoors(schematic);
            CustomItemManager.DestoryItems(schematic);
            CameraManager.DestroyCameras(schematic);
            LockerManager.DestroyLockers(schematic);

            foreach (GameObject obj in schematic.AttachedBlocks)
            {
                if (obj == null)
                    continue;

                if (obj.TryGetComponent<FluidTank>(out var fluidTank))
                    UnityEngine.Object.Destroy(fluidTank);

                if (obj.TryGetComponent<KillAreaDetector>(out var killArea))
                    UnityEngine.Object.Destroy(killArea);

                if (obj.TryGetComponent<Teleporter>(out var teleporter))
                    UnityEngine.Object.Destroy(teleporter);
            }

            if (AudioApi.AudioPlayers.TryGetValue(schematic, out List<AudioPlayer> audioPlayers))
            {
                foreach (AudioPlayer audioPlayer in audioPlayers)
                {
                    audioPlayer.RemoveAllClips();
                    audioPlayer.Destroy();
                }

                AudioApi.AudioPlayers.Remove(schematic);
            }

            response = $"Unloaded objects for {schematicName}";
            return true;
        }
    }
}