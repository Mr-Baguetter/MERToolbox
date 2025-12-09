using System.Linq;
using MapGeneration.RoomConnectors;
using Mirror;
using UnityEngine;

namespace MERToolbox.API.Helpers
{
    public class PrefabManager
    {
        public static GameObject SimpleBoxes { get; set; }
        public static GameObject PipesShort { get; set; }
        public static GameObject BoxesLadder { get; set; }
        public static GameObject TankSupportedShelf { get; set; }
        public static GameObject AngledFences { get; set; }
        public static GameObject HugeOrangePipes { get; set; }
        public static GameObject PipesLong { get; set; }

        public static void RegisterPrefabs()
        {
            int total = 0;
            foreach (GameObject value in NetworkClient.prefabs.Values.ToArray())
            {
                // Clutter Prefabs
                if (value.GetComponent<SpawnableClutterConnector>())
                {
                    switch (value.name)
                    {
                        case "Simple Boxes Open Connector":
                            SimpleBoxes = value;
                            total++;
                            break;

                        case "Pipes Short Open Connector":
                            PipesShort = value;
                            total++;
                            break;

                        case "Boxes Ladder Open Connector":
                            BoxesLadder = value;
                            total++;
                            break;

                        case "Tank-Supported Shelf Open Connector":
                            TankSupportedShelf = value;
                            total++;
                            break;

                        case "Angled Fences Open Connector":
                            AngledFences = value;
                            total++;
                            break;

                        case "Huge Orange Pipes Open Connector":
                            HugeOrangePipes = value;
                            total++;
                            break;

                        case "Pipes Long Open Connector":
                            PipesLong = value;
                            total++;
                            break;
                    }
                }
            }

            LogManager.Info($"Registered {total} Prefabs");
        }
    }
}