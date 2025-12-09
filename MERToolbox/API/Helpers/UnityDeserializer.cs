using MERToolbox.API.Data;
using MERToolbox.API.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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
                                case Enums.BlockTypes.Door:
                                    DoorData door = new()
                                    {
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId,
                                        FileName = Path.GetFileNameWithoutExtension(fileName)
                                    };
                                    
                                    door.DeserializeProperties(block.Properties);
                                    ConfigManager.DoorData.Add(door);
                                    LogManager.Info($"Loaded DoorData {Path.GetFileNameWithoutExtension(fileName)}");
                                    break;

                                case Enums.BlockTypes.Clutter:
                                    ClutterSchematic schematic = new()
                                    {
                                        Position = block.Position,
                                        Rotation = block.Rotation,
                                        Name = block.Name,
                                        ObjectId = block.ObjectId,
                                        ParentId = block.ParentId,
                                        FileName = Path.GetFileNameWithoutExtension(fileName)
                                    };

                                    schematic.DeserializeProperties(block.Properties);
                                    ConfigManager.ClutterSchematics.Add(schematic);
                                    LogManager.Info($"Loaded ClutterData {Path.GetFileNameWithoutExtension(fileName)}");
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}