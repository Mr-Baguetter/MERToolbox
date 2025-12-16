using MERToolbox.API.Data;
using MERToolbox.API.Data.SerializedData;
using MERToolbox.API.Enums;
using MERToolbox.API.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using MERTAudioPlayer = MERToolbox.API.Data.AudioPlayer; 
using UnityEngine;
using MapGeneration.Distributors;

namespace MERToolbox.API.Helpers
{
    public class UnityDeserializer
    {
        public static void Load(string name)
        {
            if (!ConfigManager.Is(name))
            {
                LogManager.Warn($"{name} directory does not exist. Creating...");
                Directory.CreateDirectory(Path.Combine(ConfigManager.Dir, name));
            }

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };
            options.Converters.Add(new Vector3Converter());

            foreach (string fileName in ConfigManager.List(name, "*.json"))
            {
                string fileContent = File.ReadAllText(fileName);
                switch (name)
                {
                    case "UnityData":
                        List<SerializedBlock> blocks = JsonSerializer.Deserialize<List<SerializedBlock>>(fileContent, options);
                        foreach (SerializedBlock block in blocks)
                        {
                            switch (block.BlockType)
                            {
                                case BlockTypes.Door:
                                    DoorData door = new()
                                    {
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId,
                                        FileName = Path.GetFileNameWithoutExtension(fileName)
                                    };
                                    
                                    if (door.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.DoorData.Add(door);
                                        LogManager.Info($"Loaded DoorData {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load DoorData {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");
                                    break;

                                case BlockTypes.Clutter:
                                    ClutterSchematic schematic = new()
                                    {
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        Name = block.Name,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId,
                                        FileName = Path.GetFileNameWithoutExtension(fileName)
                                    };

                                    if (schematic.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.ClutterSchematics.Add(schematic);
                                        LogManager.Info($"Loaded ClutterData {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load ClutterData {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");
                                    break;

                                case BlockTypes.CustomItemSpawner:
                                    CustomItemSpawn item = new()
                                    {
                                        FileName = Path.GetFileNameWithoutExtension(fileName),
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId
                                    };

                                    if (item.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.CustomItemSpawns.Add(item);
                                        LogManager.Debug($"{block.Position}");
                                        LogManager.Info($"Loaded CustomItem spawner {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load CustomItem spawner {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");
                                    break;

                                case BlockTypes.AudioPlayer:
                                    MERTAudioPlayer audio = new()
                                    {
                                        FileName = Path.GetFileNameWithoutExtension(fileName),
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId
                                    };

                                    if (audio.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.AudioPlayers.Add(audio);
                                        LogManager.Info($"Loaded AudioPlayer {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load AudioPlayer {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");
                                        
                                    break;

                                case BlockTypes.Camera:
                                    CameraData camera = new()
                                    {
                                        FileName = Path.GetFileNameWithoutExtension(fileName),
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId
                                    };

                                    if (camera.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.Cameras.Add(camera);
                                        LogManager.Info($"Loaded Camera {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load Camera {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");

                                    break;

                                case BlockTypes.Locker:
                                    LockerData locker = new()
                                    {
                                        FileName = Path.GetFileNameWithoutExtension(fileName),
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId
                                    };

                                    if (locker.TryDeserializeProperties(block.Properties))
                                    {
                                        ConfigManager.Lockers.Add(locker);
                                        LogManager.Info($"Loaded Locker {Path.GetFileNameWithoutExtension(fileName)}");
                                    }
                                    else
                                        LogManager.Error($"Failed to load Locker {block.ObjectId} in {Path.GetFileNameWithoutExtension(fileName)}");
                                    
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}