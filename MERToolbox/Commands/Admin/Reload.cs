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
using System.Text;
using UnityEngine;
using MERTAudioPlayer = MERToolbox.API.Data.AudioPlayer;

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
                    DoorSpawner.DestroyDoors(schematic);
                    CustomItemManager.DestoryItems(schematic);
                    CameraManager.DestroyCameras(schematic);
                    LockerManager.DestroyLockers(schematic);

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
                        {
                            audioPlayer.RemoveAllClips();
                            audioPlayer.Destroy();
                        }

                        AudioApi.AudioPlayers.Remove(schematic);
                    }
                }

                ConfigManager.DoorData.Clear();
                ConfigManager.ClutterSchematics.Clear();
                ConfigManager.KillAreas.Clear();
                ConfigManager.TankData.Clear();
                ConfigManager.TeleporterData.Clear();
                ConfigManager.CustomItemSpawns.Clear();
                ConfigManager.AudioPlayers.Clear();
                ConfigManager.Cameras.Clear();
                ConfigManager.Lockers.Clear();

                UnityDeserializer.Load("UnityData");
                ConfigManager.CreateAndLoad("KillAreaData");
                ConfigManager.CreateAndLoad("TankData");
                ConfigManager.CreateAndLoad("TeleporterData");

                foreach (SchematicObject schematic in MERHandler.LoadedSchematicObjects.ToArray())
                {
                    DoorSpawner.SpawnDoor(schematic);
                    CustomItemManager.TrySpawnItems(schematic);
                    ClutterManager.GenerateClutter(schematic, out _);
                    CameraManager.SpawnCamera(schematic);
                    LockerManager.SpawnLockers(schematic);
                    Plugin.Instance.AudioApi.PlayAudio(schematic);
                }

                StringBuilder sb = new();
                sb.AppendLine("Successfully reloaded MERToolbox configuration");
                sb.AppendLine($"{Plugin.LoadedAmount()}");
                response = sb.ToString();
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