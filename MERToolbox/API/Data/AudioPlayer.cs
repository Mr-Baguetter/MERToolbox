using System.Collections.Generic;
using System.Text.Json;
using MERToolbox.API.Helpers;
using UnityEngine;
using AudioAPI = AudioPlayer;

namespace MERToolbox.API.Data
{
    public class AudioPlayer
    {
        public string FileName { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public int ObjectId { get; set; }
        public int ParentId { get; set; }

        public string AudioPath { get; set; }
        public float Volume { get; set; }
        public float AudibleDistance { get; set; }
        public bool Loop { get; set; }

        public AudioAPI Player { get; set; }

        public void DeserializeProperties(Dictionary<string, object> properties) =>
            TryDeserializeProperties(properties);

        public bool TryDeserializeProperties(Dictionary<string, object> properties)
        {
            if (properties == null)
                return false;

            if (properties.TryGetValue("AudioPath", out object pathObj))
            {
                if (pathObj is JsonElement pathElement)
                {
                    this.AudioPath = pathElement.GetString();
                    LogManager.Debug($"Deserialized Audio Path: {this.AudioPath}");
                }
            }
            else
            {
                LogManager.Warn("AudioPath property not found");
                return false;
            }
            
            if (properties.TryGetValue("Volume", out object volumeObj))
            {
                if (volumeObj is JsonElement volumeElement)
                {
                    this.Volume = volumeElement.GetSingle();
                    LogManager.Debug($"Deserialized Volume: {this.Volume}");
                }
            }
            else
            {
                LogManager.Warn("Volume property not found");
                return false;
            }

            if (properties.TryGetValue("AudibleDistance", out object distanceObj))
            {
                if (distanceObj is JsonElement distanceElement)
                {
                    this.AudibleDistance = distanceElement.GetSingle();
                    LogManager.Debug($"Deserialized Audible Distance: {this.AudibleDistance}");
                }
            }
            else
            {
                LogManager.Warn("AudibleDistance property not found");
                return false;
            }

            if (properties.TryGetValue("Loop", out object loopObj))
            {
                if (loopObj is JsonElement loopElement)
                {
                    this.Loop = loopElement.GetBoolean();
                    LogManager.Debug($"Deserialized Loop: {this.Loop}");
                }
            }
            else
            {
                LogManager.Warn("Loop property not found");
                return false;
            }

            return true;
        }
    }
}