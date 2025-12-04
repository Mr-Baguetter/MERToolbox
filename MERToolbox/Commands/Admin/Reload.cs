using CommandSystem;
using MERToolbox.API;
using MERToolbox.API.Components;
using MERToolbox.API.Data;
using MERToolbox.API.Helpers;
using MERToolbox.Commands.Base;
using Mirror;
using ProjectMER.Features.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MERToolbox.Commands.Admin
{
    public class Reload : SubCommandBase
    {
        public override string Name => "reload";
        public override string Description { get; } = "Reloads the MERToolbox configuration files.";
        public override string RequiredPermission { get; } = "mertoolbox.reload";

        public override bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                foreach (SchematicObject schematic in MERHandler.LoadedSchematicObjects.ToArray())
                {
                    if (schematic == null)
                        continue;

                    ClutterManager.RemoveClutter(schematic);

                    foreach (var doorEntry in DoorSpawner.DoorIDs.Where(d => d.Value == schematic).ToList())
                    {
                        LogManager.Debug("Destroying Door.");
                        if (doorEntry.Key != null)
                            NetworkServer.Destroy(doorEntry.Key);

                        DoorSpawner.DoorIDs.Remove(doorEntry.Key);
                    }

                    foreach (GameObject obj in schematic.AttachedBlocks)
                    {
                        if (obj == null)
                            continue;

                        if (obj.TryGetComponent<FluidTank>(out var fluidTank))
                            Object.Destroy(fluidTank);

                        if (obj.TryGetComponent<KillAreaDetector>(out var killArea))
                            Object.Destroy(killArea);

                        if (obj.TryGetComponent<Teleporter>(out var teleporter))
                            Object.Destroy(teleporter);
                    }

                    if (AudioApi.AudioPlayers.TryGetValue(schematic, out List<AudioPlayer> audioPlayers))
                    {
                        foreach (AudioPlayer audioPlayer in audioPlayers)
                            audioPlayer.Destroy();

                        AudioApi.AudioPlayers.Remove(schematic);
                    }
                }

                ConfigManager.DoorData.Clear();
                ConfigManager.ClutterSchematics.Clear();
                ConfigManager.KillAreas.Clear();
                ConfigManager.TankData.Clear();
                ConfigManager.TeleporterData.Clear();
                ConfigManager.AudioPathing.Clear();

                UnityDeserializer.Load("UnityData");
                ConfigManager.CreateAndLoad("KillAreaData");
                ConfigManager.CreateAndLoad("TankData");
                ConfigManager.CreateAndLoad("TeleporterData");
                ConfigManager.CreateAndLoad("SoundData");

                foreach (SchematicObject schematic in MERHandler.LoadedSchematicObjects.ToArray())
                {
                    DoorSpawner.SpawnDoor(schematic);
                    ClutterManager.GenerateClutter(schematic, out _);
                }

                response = $"Successfully reloaded MERToolbox configuration. Respawn the Schematic to apply changes. \n {Plugin.LoadedAmount()}";
                return true;
            }
            catch (System.Exception ex)
            {
                response = $"Failed to reload: {ex.Message}";
                LogManager.Error($"Reload command failed: {ex}");
                return false;
            }
        }
    }
}