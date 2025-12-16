using System;
using System.Collections.Generic;
using AdminToys;
using MERToolbox.API.Data;
using Mirror;
using ProjectMER.Features.Objects;
using UnityEngine;
using CameraType = ProjectMER.Features.Enums.CameraType;

namespace MERToolbox.API.Helpers
{
    public class CameraManager
    {
        public static Dictionary<SchematicObject, List<CameraData>> CamerasBySchematic = [];

        public static GameObject GetCameraPrefab(CameraType type)
        {
            return type switch
            {
                CameraType.Ez => ProjectMER.Features.PrefabManager.CameraEz.gameObject,
                CameraType.EzArm => ProjectMER.Features.PrefabManager.CameraEzArm.gameObject,
                CameraType.Hcz => ProjectMER.Features.PrefabManager.CameraHcz.gameObject,
                CameraType.Lcz => ProjectMER.Features.PrefabManager.CameraLcz.gameObject,
                CameraType.Sz => ProjectMER.Features.PrefabManager.CameraSz.gameObject,
                _ => throw new InvalidOperationException(),
            };
        }

        public static void SpawnCamera(SchematicObject schematic)
        {
            foreach (CameraData camera in ConfigManager.Cameras.ToArray())
            {
                if (!CamerasBySchematic.ContainsKey(schematic))
                    CamerasBySchematic[schematic] = [];

                if (schematic.Name == camera.FileName)
                {
                    GameObject cameraPrefab = UnityEngine.Object.Instantiate(GetCameraPrefab(camera.CameraType));
                    ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, camera.Position, camera.Rotation, out Vector3 position, out Quaternion rotation);
                    camera.GameObject = cameraPrefab;
                    cameraPrefab.transform.position = position;
                    cameraPrefab.transform.rotation = rotation;
                    CamerasBySchematic[schematic].Add(camera);
                    NetworkServer.Spawn(cameraPrefab);

                    if (cameraPrefab.TryGetComponent<Scp079CameraToy>(out var cam))
                    {
                        cam.Label = camera.Label;
                        cam.ZoneIcon = camera.ZoneIcon;
                    }
                }
            }   
        }

        public static void DestroyCameras(SchematicObject schematic)
        {
            if (!CamerasBySchematic.ContainsKey(schematic))
                return;

            foreach (CameraData camera in CamerasBySchematic[schematic].ToArray())
                NetworkServer.Destroy(camera.GameObject);
        }
    }
}