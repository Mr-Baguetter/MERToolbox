using MEC;
using ProjectMER.Features.Objects;
using MERToolbox.API.Data;
using System.Collections.Generic;
using System.IO;
using MERToolbox.API.Helpers;
using UnityEngine;
using MERTAudioPlayer = MERToolbox.API.Data.AudioPlayer;
using System;

namespace MERToolbox.API
{
    public class AudioApi
    {
        public static Dictionary<SchematicObject, List<AudioPlayer>> AudioPlayers { get; set; } = [];

        public static float Clamp(float? value, float min, float max) => 
            (float)((value < min) ? min : (value > max) ? max : value);

        public void PlayAudio(SchematicObject schematic)
        {
            if (ConfigManager.AudioPlayers == null || ConfigManager.AudioPlayers.IsEmpty())
            {
                LogManager.Error("SoundLists is null or empty.");
                return;
            }

            foreach (MERTAudioPlayer player in ConfigManager.AudioPlayers.ToArray())
            {
                if (schematic.Name == player.FileName)
                {
                    LogManager.Debug($"Audio API is enabled!");

                    if (string.IsNullOrEmpty(player.AudioPath))
                    {
                        LogManager.Error($"Audio path is null please fill out the config properly.");
                        continue;
                    }

                    ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, player.Position, player.Rotation, out Vector3 position, out Quaternion rotation);
                    LogManager.Debug($"Successfully loaded audio path {player.AudioPath}");

                    string guid = Guid.NewGuid().ToString();
                    AudioPlayer audioPlayer = AudioPlayer.Create($"Global_Audio_{guid}", onIntialCreation: (p) =>
                    {
                        Speaker speaker = p.AddSpeaker("Main", position, isSpatial: true, maxDistance: player.AudibleDistance);
                    });

                    float volume = Clamp(player.Volume, 1f, 100f)/100;
                    string clip = $"sound_{guid}";
                    audioPlayer.AddClip(clip, volume, player.Loop);
                    AudioClipStorage.LoadClip(player.AudioPath, $"Global_Audio_{guid}");
                    player.Player = audioPlayer;

                    LogManager.Debug($"Playing {Path.GetFileName(player.AudioPath)}");
                    LogManager.Debug($"Audio should have been played.");
                    if (AudioPlayers.ContainsKey(schematic))
                        AudioPlayers[schematic].Add(audioPlayer);
                    else
                        AudioPlayers.Add(schematic, [audioPlayer]);
                }
            }
        }
    }
}