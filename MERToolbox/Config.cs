using MERToolbox.API.Data;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace MERToolbox
{
    public class Config
    {
        /// <summary>
        /// A collection of clutter schematic definitions used for adding clutter to schematics
        /// </summary>
        public Dictionary<string, List<ClutterSchematic>> ClutterSchematics { get; set; } = new()
        {
            ["CustomSchematic-GameObject"] =
            [
                new ClutterSchematic { Name = "Barrels", SpawnChance = 30, Rotation = Vector3.zero},
                new ClutterSchematic { Name = "Barrel", SpawnChance = 30, Rotation = Vector3.zero },
                new ClutterSchematic { Name = "Spill", SpawnChance = 30, Rotation = Vector3.zero },
                new ClutterSchematic { Name = "RoadCone", SpawnChance = 30, Rotation = Vector3.zero }
            ]
        };

        public List<string> ClutterAreaNames { get; set; } =
        [
            "clutter",
            "clutterarea"
        ];
        
        [Description("Sound data.")]
        public List<SoundList> AudioPathing { get; set; } =
        [
            new()
        ];

        public List<KillArea> KillAreas { get; set; } =
        [
            new()
        ];

        public List<TankData> TankData { get; set; } =
        [
            new()
        ];

        public List<DoorData> DoorData { get; set; } =
        [
            new()
        ];

        public List<TeleporterData> TeleporterData { get; set; } =
        [
            new()
        ];
    }

    /// <summary>
    /// Represents a clutter schematic with its spawn chance
    /// </summary>
    public class ClutterSchematic
    {
        /// <summary>
        /// The schematic name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The spawn chance percentage
        /// </summary>
        public float SpawnChance { get; set; }

        /// <summary>
        /// The Rotation of the Clutter when spawned
        /// </summary> 
        public Vector3 Rotation { get; set; }

        /*
        public void Deconstruct(out string name, out float spawnChance, out Quaternion rotation)
        {
            name = Name;
            spawnChance = SpawnChance;
            rotation = Quaternion.Euler(Rotation);
        }
        */
    }
}
