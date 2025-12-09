using MEC;
using ProjectMER.Features.Objects;
using MERToolbox.API.Data;
using System.Collections.Generic;
using System.IO;
using MERToolbox.API.Helpers;
using UnityEngine;

namespace MERToolbox.API
{
    public class AudioApi
    {
        public static Dictionary<SchematicObject, List<CoroutineHandle>> CoroutineHandles { get; set; } = [];

        public static Dictionary<SchematicObject, List<AudioPlayer>> AudioPlayers { get; set; } = [];

        public List<SoundList> SoundLists { get; set; }

        public void SetSoundLists(List<SoundList> soundLists)
        {
            SoundLists = soundLists ?? [];
            LogManager.Debug($"Set {SoundLists.Count} sound configurations in AudioApi");
        }
        
        public static float Clamp(float? value, float min, float max)
        {
            return (float)((value < min) ? min : (value > max) ? max : value);
        }

        public void PlayAudio(SchematicObject Schematic)
        {
            if (SoundLists == null || SoundLists.IsEmpty())
            {
                LogManager.Error("SoundLists is null or empty.");
                return;
            }

            foreach (GameObject SpawnedObject in Schematic.AttachedBlocks)
            {
                if (SpawnedObject == null)
                    continue;

                foreach (SoundList soundList in SoundLists)
                {
                    if (SpawnedObject.name == soundList.PrimitiveName)
                    {
                        LogManager.Debug($"Audio API is enabled!");

                        if (string.IsNullOrEmpty(soundList.AudioPath))
                        {
                            LogManager.Error($"Audio path is null please fill out the config properly.");
                            continue;
                        }

                        Vector3 Coords = SpawnedObject.transform.position;
                        LogManager.Debug($"Successfully loaded audio path {soundList.AudioPath}");

                        AudioPlayer audioPlayer = AudioPlayer.CreateOrGet($"Global_Audio_{soundList.PrimitiveName}", onIntialCreation: (p) =>
                        {
                            Speaker speaker = p.AddSpeaker("Main", Coords, isSpatial: true, maxDistance: soundList.AudibleDistance);
                        });

                        float volume = Clamp(soundList.SoundVolume, 1f, 100f);
                        audioPlayer.AddClip($"sound_{soundList.PrimitiveName}", volume, soundList.Loop);
                        AudioClipStorage.LoadClip(soundList.AudioPath, $"sound_{soundList.PrimitiveName}");

                        LogManager.Debug($"Playing {Path.GetFileName(soundList.AudioPath)}");
                        LogManager.Debug($"Audio should have been played.");
                        if (AudioPlayers.ContainsKey(Schematic))
                            AudioPlayers[Schematic].Add(audioPlayer);
                        else
                            AudioPlayers.Add(Schematic, [audioPlayer]);

                        if (CoroutineHandles.ContainsKey(Schematic))
                            CoroutineHandles[Schematic].Add(Timing.RunCoroutine(UpdateAudioLocation(audioPlayer, Coords)));
                        else
                            CoroutineHandles.Add(Schematic, [Timing.RunCoroutine(UpdateAudioLocation(audioPlayer, Coords))]);
                    }
                }
            }
        }

        internal IEnumerator<float> UpdateAudioLocation(AudioPlayer audioPlayer, Vector3 coords)
        {
            while (true)
            {
                if (audioPlayer is null)
                    yield break;

                if (coords == Vector3.zero)
                    yield break;

                audioPlayer.SetSpeakerPosition(audioPlayer.Name, coords);
                yield return Timing.WaitForSeconds(1);
            }
        }
    }
}