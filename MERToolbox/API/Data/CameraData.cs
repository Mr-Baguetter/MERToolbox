using System.Collections.Generic;
using System.Text.Json;
using MapGeneration;
using MERToolbox.API.Helpers;
using UnityEngine;
using CameraType = ProjectMER.Features.Enums.CameraType;

namespace MERToolbox.API.Data
{
    public class CameraData
    {
        public string FileName { get; set; }

        public Vector3 Position { get; set; }

        public Vector3 Rotation { get; set; }

        public int ObjectId { get; set; }
        
        public int ParentId { get; set; }

        public CameraType CameraType { get; set; }
        public string Label { get; set; }
	    public FacilityZone ZoneIcon { get; set; }

        internal GameObject GameObject { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            if (properties.TryGetValue("CameraType", out object cameraTypeObj))
            {
                if (cameraTypeObj is JsonElement cameraTypeElement)
                {
                    this.CameraType = (CameraType)cameraTypeElement.GetInt32();
                    LogManager.Debug($"Deserialized Camera Type: {this.CameraType}");
                }
            }
            else
            {
                LogManager.Warn("SpawnChance property not found");
                return false;
            }

            if (properties.TryGetValue("CameraLabel", out object labelObj) && labelObj is JsonElement labelElement)
            {
                this.Label = labelElement.GetString();
                LogManager.Debug($"Deserialized CameraLabel Type: {this.Label}");
            }
            else
            {
                LogManager.Warn("CameraLabel property not found");
                return false;
            }

            if (properties.TryGetValue("ZoneIcon", out object zoneObj) && zoneObj is JsonElement zoneElement)
            {
                this.ZoneIcon = (FacilityZone)zoneElement.GetInt32();
                LogManager.Debug($"Deserialized ZoneIcon Type: {this.ZoneIcon}");
            }
            else
            {
                LogManager.Warn("ZoneIcon property not found");
                return false;
            }

            return true;
        }
    }
}