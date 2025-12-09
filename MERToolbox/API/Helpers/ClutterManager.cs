using LabApi.Features.Wrappers;
using MERToolbox.API.Data;
using MERToolbox.API.Enums;
using Mirror;
using ProjectMER.Features.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using Random = UnityEngine.Random;

namespace MERToolbox.API.Helpers
{
    public class ClutterManager
    {
        /// <summary>
        /// Tracks the currently spawned clutter objects for each schematic.
        /// </summary>
        /// <value>
        /// A <see cref="Dictionary{TKey, TValue}"/> where the key is the 
        /// <see cref="SchematicObject"/> that owns the clutter, and the value is a 
        /// <see cref="List{T}"/> of <see cref="SchematicObject"/> instances representing 
        /// the clutter objects spawned for that schematic.
        /// </value>
        public static Dictionary<SchematicObject, List<GameObject>> ActiveClutter { get; set; } = [];

        /// <summary>
        /// Retrieves the list of active clutter objects associated with the specified schematic.
        /// </summary>
        /// <param name="schematic">
        /// The <see cref="SchematicObject"/> whose active clutter objects should be retrieved.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="SchematicObject"/> instances currently tracked
        /// for the given schematic. 
        /// </returns>
        public static List<GameObject> GetActiveClutter(SchematicObject schematic) => ActiveClutter[schematic];

        /// <summary>
        /// Attempts to retrieve the list of active clutter objects associated with the specified schematic.
        /// </summary>
        /// <param name="schematic">
        /// The <see cref="SchematicObject"/> whose active clutter objects should be retrieved.
        /// </param>
        /// <param name="clutter">
        /// When this method returns, contains the list of <see cref="SchematicObject"/> instances
        /// associated with the schematic if the key was found; otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the schematic exists in <see cref="ActiveClutter"/> and the associated
        /// clutter list was retrieved; otherwise, <c>false</c>.
        /// </returns>
        public static bool TryGetActiveClutter(SchematicObject schematic, out List<GameObject> clutter) => ActiveClutter.TryGetValue(schematic, out clutter);

        public static GameObject GetClutterPrefab(ClutterType type)
        {
            return type switch
            {
                ClutterType.SimpleBoxes => PrefabManager.SimpleBoxes,
                ClutterType.PipesShort => PrefabManager.PipesShort,
                ClutterType.BoxesLadder => PrefabManager.BoxesLadder,
                ClutterType.TankSupportedShelf => PrefabManager.TankSupportedShelf,
                ClutterType.AngledFences => PrefabManager.AngledFences,
                ClutterType.HugeOrangePipes => PrefabManager.HugeOrangePipes,
                ClutterType.PipesLongOpen => PrefabManager.PipesLong,
                _ => throw new InvalidOperationException(),
            };
        }

        /// <summary>
        /// Adds clutter to a schematic. Clutter is defined by <see cref="Config.ClutterSchematics"/>
        /// </summary>
        /// <param name="schematic"></param>
        /// <param name="spawnedClutter"></param>
        public static void GenerateClutter(SchematicObject schematic, out List<GameObject> spawnedClutter)
        {
            spawnedClutter = [];
            foreach (ClutterSchematic clutter in ConfigManager.ClutterSchematics.Where(d => d.FileName == schematic.Name))
            {
                if (Random.Range(0f, 100f) <= clutter.SpawnChance)
                {
                    GameObject clutterPrefab = UnityEngine.Object.Instantiate(GetClutterPrefab(clutter.ClutterType));
                    NetworkServer.UnSpawn(clutterPrefab);
                    ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, clutter.Position, clutter.Rotation, out Vector3 position, out Quaternion rotation);
                    clutterPrefab.transform.rotation = rotation;
                    clutterPrefab.transform.position = position;
                    NetworkServer.Spawn(clutterPrefab);
                    spawnedClutter.Add(clutterPrefab);

                    /*
                    GameObject clutterPrefab = UnityEngine.Object.Instantiate(GetClutterPrefab(clutter.ClutterType));
                    NetworkServer.UnSpawn(clutterPrefab);
                    clutter.GameObject = clutterPrefab;
                    clutterPrefab.transform.SetParent(schematic.gameObject.transform);
                    clutterPrefab.transform.localPosition = clutter.Position;
                    clutterPrefab.transform.localRotation = Quaternion.Euler(clutter.Rotation);
                    LogManager.Debug($"Spawning Clutter at {clutterPrefab.transform.position} - {schematic.Position} - {clutterPrefab.transform.localPosition}");
                    LogManager.Debug($"{clutterPrefab.transform.localRotation} - {clutterPrefab.transform.localScale} - {clutterPrefab.isStatic} - {clutter.Position}");
                    NetworkServer.Spawn(clutterPrefab);
                    spawnedClutter.Add(clutterPrefab);
                    */
                }
            }

            if (spawnedClutter.Count > 0)
                ActiveClutter[schematic] = spawnedClutter;
        }

        /// <summary>
        /// Removes clutter previously added to a schematic
        /// </summary>
        /// <param name="schematic"></param>
        public static void RemoveClutter(SchematicObject schematic)
        {
            if (!ActiveClutter.TryGetValue(schematic, out var clutterList))
                return;

            foreach (GameObject gameObject in clutterList.ToArray())
                NetworkServer.Destroy(gameObject);

            ActiveClutter.Remove(schematic);
        }
    }
}