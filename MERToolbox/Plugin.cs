global using Logger = LabApi.Features.Console.Logger;
using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MERToolbox.API;
using MERToolbox.API.Helpers;
using System;
using System.Collections.Generic;

// & "C:\Program Files\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe" MERToolbox.csproj
namespace MERToolbox
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "MERToolbox";
        public override string Description => ":3";
        public override string Author => "Mr. Baguetter";
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
        public override Version Version => new(1, 2, 2, 0);
        public override LoadPriority Priority => LoadPriority.Low;
        public static Plugin Instance;
        internal AudioApi AudioApi;
        internal Harmony _harmony;

        public override void Enable()
        {
            Instance = this;
            AudioApi = new();
            _harmony = new($"MrBaguetter-MERToolbox-{DateTime.Now}");
            _harmony.PatchAll();

            UnityDeserializer.Load("UnityData");
            ConfigManager.CreateAndLoad("KillAreaData");
            ConfigManager.CreateAndLoad("TankData");
            ConfigManager.CreateAndLoad("TeleporterData");
            ConfigManager.Create("Logs");
            ConfigManager.Create("Audio");

            LogManager.Info(LoadedAmount());
            
            MERHandler.Register();
            Handler.Register();
            Config.AudioPath = ConfigManager.AudioPath;
            SaveConfig();
        }

        public override void Disable()
        {
            MERHandler.Unregister();
            Handler.Unregister();
            _harmony.UnpatchAll();

            _harmony = null;
            AudioApi = null;
            Instance = null;
        }

        public static string LoadedAmount()
        {
            List<string> parts = [];
            void AddIfAny(string label, int count)
            {
                if (count > 0)
                    parts.Add($"{count} {label}");
            }

            AddIfAny("Doors", ConfigManager.DoorData.Count);
            AddIfAny("ClutterSchematics", ConfigManager.ClutterSchematics.Count);
            AddIfAny("KillAreas", ConfigManager.KillAreas.Count);
            AddIfAny("AudioPlayers", ConfigManager.AudioPlayers.Count);
            AddIfAny("TankData", ConfigManager.TankData.Count);
            AddIfAny("Teleporters", ConfigManager.TeleporterData.Count);
            AddIfAny("CustomItem Spawns", ConfigManager.CustomItemSpawns.Count);
            AddIfAny("Cameras", ConfigManager.Cameras.Count);
            AddIfAny("Lockers", ConfigManager.Lockers.Count);

            return $"Loaded {string.Join(", ", parts)} ({ConfigManager.DoorData.Count + ConfigManager.ClutterSchematics.Count + ConfigManager.KillAreas.Count + ConfigManager.AudioPlayers.Count + ConfigManager.TankData.Count + ConfigManager.TeleporterData.Count + ConfigManager.CustomItemSpawns.Count + ConfigManager.Cameras.Count + ConfigManager.Lockers.Count} Total files)";
        }
    }
}