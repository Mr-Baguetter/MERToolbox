using System.Collections.Generic;
using System.Linq;
using ProjectMER.Features.Objects;
using UnityEngine;
using Mirror;
using ProjectMER.Features;

namespace MERToolbox.API.Helpers
{
    public static class ClutterManager
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
        public static Dictionary<SchematicObject, List<SchematicObject>> ActiveClutter { get; set; } = [];

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
        public static List<SchematicObject> GetActiveClutter(SchematicObject schematic) => ActiveClutter[schematic];

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
        public static bool TryGetActiveClutter(SchematicObject schematic, out List<SchematicObject> clutter) => ActiveClutter.TryGetValue(schematic, out clutter);

        /// <summary>
        /// Adds clutter to a schematic. Clutter is defined by <see cref="Config.ClutterSchematics"/>
        /// </summary>
        /// <param name="schematic"></param>
        /// <param name="spawnedClutter"></param>
        public static void GenerateClutter(SchematicObject schematic, out List<SchematicObject> spawnedClutter)
        {
            spawnedClutter = [];
            if (!Plugin.Instance.Config.ClutterSchematics.TryGetValue(schematic.Name, out List<ClutterSchematic> clutterlist))
                return;

            List<GameObject> clutterAreas = schematic.AttachedBlocks.Where(b => b != null && Plugin.Instance.Config.ClutterAreaNames.Contains(b.name)).ToList();
            foreach (GameObject block in clutterAreas)
            {
                Vector3 position = block.transform.position;
                foreach (ClutterSchematic clutter in clutterlist)
                {
                    if (Random.Range(0f, 100f) <= clutter.SpawnChance)
                    {
                        SchematicObject clutterObject = ObjectSpawner.SpawnSchematic(clutter.Name, position);
                        if (clutterObject == null)
                            continue;

                        clutterObject.Rotation = clutter.Rotation != Vector3.zero ? Quaternion.Euler(clutter.Rotation) : block.transform.rotation;
                        spawnedClutter.Add(clutterObject);
                        foreach (GameObject spawnedObject in clutterObject.AttachedBlocks.ToList())
                        {
                            if (spawnedObject == null)
                                continue;

                            spawnedObject.transform.SetParent(schematic.transform, true);
                            schematic._attachedBlocks.Add(spawnedObject);
                        }
                    }
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

            foreach (SchematicObject clutter in clutterList)
            {
                if (clutter?.AttachedBlocks == null)
                    continue;

                foreach (GameObject go in clutter.AttachedBlocks.ToList())
                {
                    if (go == null)
                        continue;
                    
                    NetworkServer.Destroy(go);
                }

                clutter.Destroy();
            }

            ActiveClutter.Remove(schematic);
        }
    }
}
