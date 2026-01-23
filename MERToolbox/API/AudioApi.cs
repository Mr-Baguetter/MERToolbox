using LabApi.Features.Wrappers;
using MEC;
using MERToolbox.API.Data;
using MERToolbox.API.Helpers;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MERTAudioPlayer = MERToolbox.API.Data.AudioPlayer;

namespace MERToolbox.API
{
    public class AudioApi
    {
        public static Dictionary<SchematicObject, List<AudioPlayer>> AudioPlayers { get; set; } = [];

        public static float Clamp(float? value, float min, float max) => 
            (float)((value < min) ? min : (value > max) ? max : value);

        public void PlayAudio(SchematicObject schematic)
        {
            if (ConfigManager.AudioPlayers.IsEmpty())
            {
                LogManager.Error("No AudioPlayers are loaded.");
                return;
            }

            foreach (MERTAudioPlayer player in ConfigManager.AudioPlayers.ToArray())
            {
                if (schematic.Name == player.FileName)
                {
                    if (player.AudioPath.Contains(".config/"))
                    {
                        HandleFullPath(schematic, player);
                    }
                    else if (File.Exists(Path.Combine(Plugin.Instance.Config.AudioPath, player.AudioPath)))
                    {
                        HandleFile(schematic, player, Path.Combine(Plugin.Instance.Config.AudioPath, player.AudioPath));
                    }
                    else
                        LogManager.Warn($"File does not exist at {player.AudioPath}");
                }
            }
        }

        public void HandleFullPath(SchematicObject schematic, MERTAudioPlayer player)
        {
            LogManager.Debug("Audio API is enabled!");

            ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, player.Position, player.Rotation, out Vector3 position, out Quaternion rotation);
            LogManager.Debug($"Successfully loaded audio path {player.AudioPath}");

            string guid = Guid.NewGuid().ToString();
            string clipName = $"sound_{guid}";
            
            AudioClipStorage.LoadClip(player.AudioPath, clipName);
            AudioPlayer audioPlayer = AudioPlayer.Create($"{schematic.Name}_{guid}", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", position, isSpatial: true, maxDistance: player.AudibleDistance);
            });

            float volume = Clamp(player.Volume, 1f, 100f)/100;
            audioPlayer.AddClip(clipName, volume, player.Loop);
            player.Player = audioPlayer;

            LogManager.Debug($"Playing {Path.GetFileName(player.AudioPath)}");
            if (AudioPlayers.ContainsKey(schematic))
            {
                AudioPlayers[schematic].Add(audioPlayer);                
            }
            else
                AudioPlayers.Add(schematic, [audioPlayer]);
        }

        public void HandleFile(SchematicObject schematic, MERTAudioPlayer player, string path)
        {
            LogManager.Debug("Audio API is enabled!");

            ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, player.Position, player.Rotation, out Vector3 position, out Quaternion rotation);
            LogManager.Debug($"Successfully loaded audio path {path}");

            string guid = Guid.NewGuid().ToString();
            string clipName = $"sound_{guid}";
            
            AudioClipStorage.LoadClip(path, clipName);
            AudioPlayer audioPlayer = AudioPlayer.Create($"{schematic.Name}_{guid}", onIntialCreation: (p) =>
            {
                Speaker speaker = p.AddSpeaker("Main", position, isSpatial: true, maxDistance: player.AudibleDistance);
            });

            float volume = Clamp(player.Volume, 1f, 100f)/100;
            audioPlayer.AddClip(clipName, volume, player.Loop);
            player.Player = audioPlayer;

            LogManager.Debug($"Playing {Path.GetFileName(path)}");
            if (AudioPlayers.ContainsKey(schematic))
            {
                AudioPlayers[schematic].Add(audioPlayer);                
            }
            else
                AudioPlayers.Add(schematic, [audioPlayer]);
        }
    }
}