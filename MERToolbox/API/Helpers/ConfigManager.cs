using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Yaml;
using MERToolbox.API.Data;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MERToolbox.API.Helpers
{
    internal class ConfigManager
    {
        public static List<SoundList> AudioPathing { get; set; } = [];
        public static List<KillArea> KillAreas { get; set; } = [];
        public static List<TankData> TankData { get; set; } = [];
        public static List<DoorData> DoorData { get; set; } = [];
        public static List<ClutterSchematic> ClutterSchematics { get; set; } = [];
        public static List<TeleporterData> TeleporterData { get; set; } = [];

        internal static string Dir = Path.Combine(PathManager.Configs.ToString(), "MERToolbox");

        public static string[] List(string localDir = "", string extension = "*.yml") =>
            Directory.GetFiles(Path.Combine(Dir, localDir), extension);
    
        public static bool Is(string localDir = "") =>
            Directory.Exists(Path.Combine(Dir, localDir));

        public static void CreateAndLoad(string name)
        {
            Create(name);
            Load(name);
        }

        public static void CalculateWorldTransform(Vector3 worldPosition, Quaternion worldRotation, Vector3 localPosition, Vector3 localRotationEuler, out Vector3 outPosition, out Quaternion outRotation)
        {
            Quaternion localRotation = Quaternion.Euler(localRotationEuler);
            outRotation = worldRotation * localRotation;
            outPosition = worldPosition + (worldRotation * localPosition);
        }

        public static void Load(string name)
        {
            foreach (string fileName in List(name))
            {
                if (Directory.Exists(fileName))
                    continue;

                string fileContent = File.ReadAllText(fileName);
                try
                {
                    switch (name)
                    {
                        case "KillAreaData":
                            KillArea killArea = YamlConfigParser.Deserializer.Deserialize<KillArea>(fileContent);
                            if (killArea is null || string.IsNullOrEmpty(killArea.PrimitiveName))
                                break;

                            KillAreas.Add(killArea);
                            LogManager.Debug($"Loaded KillArea {killArea.PrimitiveName}");
                            break;
                        
                        case "SoundData":
                            SoundList soundData = YamlConfigParser.Deserializer.Deserialize<SoundList>(fileContent);
                            if (soundData is null || string.IsNullOrEmpty(soundData.PrimitiveName))
                                break;

                            AudioPathing.Add(soundData);
                            LogManager.Debug($"Loaded AudioData {soundData.PrimitiveName}");
                            break;
                        
                        case "TankData":
                            TankData tankData = YamlConfigParser.Deserializer.Deserialize<TankData>(fileContent);
                            if (tankData is null || string.IsNullOrEmpty(tankData.PrimitiveName))
                                break;

                            TankData.Add(tankData);
                            LogManager.Debug($"Loaded TankData {tankData.PrimitiveName}");
                            break;
                        
                        case "TeleporterData":
                            TeleporterData teleporterData = YamlConfigParser.Deserializer.Deserialize<TeleporterData>(fileContent);
                            if (teleporterData is null || string.IsNullOrEmpty(teleporterData.PrimitiveName) || string.IsNullOrEmpty(teleporterData.TargetPrimitive))
                                break;

                            TeleporterData.Add(teleporterData);
                            LogManager.Debug($"Loaded TeleporterData {teleporterData.PrimitiveName}");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Failed to load {fileName} in {name} directory. Exception: {ex}");
                }
            }
        }

        public static void Create(string name)
        {
            if (!Is(name))
            {
                Directory.CreateDirectory(Path.Combine(Dir, name));
                switch (name)
                {
                    case "DoorData":
                        File.WriteAllText(Path.Combine(Dir, name, "DoorData.yml"), YamlConfigParser.Serializer.Serialize(new DoorData()));
                        LogManager.Debug("Created DoorData directory");
                        break;
                        
                    case "ClutterData":
                        File.WriteAllText(Path.Combine(Dir, name, "ClutterSchematic.yml"), YamlConfigParser.Serializer.Serialize(new ClutterSchematic()));
                        LogManager.Debug("Created Clutter directory");
                        break;

                    case "KillAreaData":
                        File.WriteAllText(Path.Combine(Dir, name, "KillArea.yml"), YamlConfigParser.Serializer.Serialize(new KillArea()));
                        LogManager.Debug("Created KillArea directory");
                        break;

                    case "SoundData":
                        File.WriteAllText(Path.Combine(Dir, name, "SoundList.yml"), YamlConfigParser.Serializer.Serialize(new SoundList()));
                        LogManager.Debug("Created SoundData directory");
                        break;

                    case "TankData":
                        File.WriteAllText(Path.Combine(Dir, name, "TankData.yml"), YamlConfigParser.Serializer.Serialize(new TankData()));
                        LogManager.Debug("Created TankData directory");
                        break;

                    case "TeleporterData":
                        File.WriteAllText(Path.Combine(Dir, name, "TeleporterData.yml"), YamlConfigParser.Serializer.Serialize(new TeleporterData()));
                        LogManager.Debug("Created TeleporterData directory");
                        break;

                    default:
                        File.Create(Path.Combine(Dir, name, $"{name}.txt"));
                        break;
                }
            }
        }
    }
}
