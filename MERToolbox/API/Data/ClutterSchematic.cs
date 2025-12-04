using MERToolbox.API.Enums;
using MERToolbox.API.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using UnityEngine;

namespace MERToolbox.API.Data
{
    public class ClutterSchematic
    {        
        public string FileName { get; set; }

        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public int ObjectId { get; set; }
        
        public int ParentId { get; set; }
        
        public float SpawnChance { get; set; }

        public ClutterType ClutterType { get; set; }

        internal GameObject GameObject { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            if (properties.TryGetValue("SpawnChance", out object spawnChanceObj))
            {
                if (spawnChanceObj is JsonElement spawnChanceElement)
                {
                    this.SpawnChance = spawnChanceElement.GetSingle();
                    LogManager.Debug($"Deserialized SpawnChance: {this.SpawnChance}");
                }
                else
                {
                    this.SpawnChance = Convert.ToSingle(spawnChanceObj);
                    LogManager.Debug($"Deserialized SpawnChance (via Convert): {this.SpawnChance}");
                }
            }
            else
            {
                LogManager.Warn("SpawnChance property not found");
                return false;
            }

            if (properties.TryGetValue("ClutterType", out object clutterTypeObj))
            {
                if (clutterTypeObj is JsonElement clutterTypeElement)
                {
                    this.ClutterType = (ClutterType)clutterTypeElement.GetInt32();
                    LogManager.Debug($"Deserialized ClutterType: {this.ClutterType}");
                }
                else
                {
                    this.ClutterType = (ClutterType)Convert.ToInt32(clutterTypeObj);
                    LogManager.Debug($"Deserialized ClutterType (via Convert): {this.ClutterType}");
                }
            }
            else
            {
                LogManager.Warn("ClutterType property not found");
                return false;
            }

            return true;
        }
    }

    public class SerializedClutter
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }
        public string Name { get; set; }
        public BlockTypes BlockType { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }
}