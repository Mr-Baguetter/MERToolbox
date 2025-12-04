global using Logger = LabApi.Features.Console.Logger;
using HarmonyLib;
using LabApi.Loader.Features.Plugins;
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
        public override Version Version => new(1, 1, 0, 0);
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
            ConfigManager.CreateAndLoad("SoundData");
            ConfigManager.CreateAndLoad("TankData");
            ConfigManager.CreateAndLoad("TeleporterData");
            ConfigManager.Create("Logs");

            LogManager.Info(LoadedAmount());
            
            AudioApi.SetSoundLists(ConfigManager.AudioPathing);
            MERHandler.Register();
            Handler.Register();
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
            AddIfAny("AudioData", ConfigManager.AudioPathing.Count);
            AddIfAny("TankData", ConfigManager.TankData.Count);
            AddIfAny("Teleporters", ConfigManager.TeleporterData.Count);

            return $"Loaded {string.Join(", ", parts)} ({ConfigManager.DoorData.Count + ConfigManager.ClutterSchematics.Count + ConfigManager.KillAreas.Count + ConfigManager.AudioPathing.Count + ConfigManager.TankData.Count + ConfigManager.TeleporterData.Count} Total files)";
        }
    }
}