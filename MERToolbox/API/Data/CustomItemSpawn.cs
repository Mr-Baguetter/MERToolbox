using System;
using System.Collections.Generic;
using System.Text.Json;
using MERToolbox.API.Data.SerializedData;
using MERToolbox.API.Enums;
using MERToolbox.API.Helpers;
using UnityEngine;

namespace MERToolbox.API.Data
{
    public class CustomItemSpawn
    {
        public string FileName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }

        public string Id { get; set; }
        public CustomItemType CustomItemType { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            if (properties.TryGetValue("CustomItemId", out object idObj))
            {
                if (idObj is JsonElement idElement)
                {
                    this.Id = idElement.GetString();
                    LogManager.Debug($"Deserialized Id: {this.Id}");
                }
            }
            else
            {
                LogManager.Warn("CustomItemId property not found");
                return false;
            }

            if (properties.TryGetValue("CustomItemType", out object customItemTypeObj))
            {
                if (customItemTypeObj is JsonElement customItemTypeElement)
                {
                    this.CustomItemType = (CustomItemType)customItemTypeElement.GetInt32();
                    LogManager.Debug($"Deserialized CustomItemType: {this.CustomItemType}");
                }
                else
                {
                    this.CustomItemType = (CustomItemType)Convert.ToInt32(customItemTypeObj);
                    LogManager.Debug($"Deserialized CustomItemType (via Convert): {this.CustomItemType}");
                }
            }
            else
            {
                LogManager.Warn("CustomItemType property not found");
                return false;
            }

            return true;
        }
    }

    public class SerializedCustomItemSpawn : SerializedBlock
    {
        public string Id { get; set; }
        public CustomItemType CustomItemType { get; set; }
    }
}