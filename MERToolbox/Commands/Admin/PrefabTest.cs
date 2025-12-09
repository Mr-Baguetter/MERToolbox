using System;
using CommandSystem;
using MERToolbox.Commands.Base;
using Mirror;
using System.Collections.Generic;
using LabApi.Features.Wrappers;
using MERToolbox.API.Enums;
using UnityEngine;
using MERToolbox.API.Helpers;

namespace MERToolbox.Commands.Admin
{
    public class PrefabTest : SubCommandBase
    {
        public override string Name => "prefab";
        public override string Description { get; } = "Spawns a specific Prefab at the users location.";
        public override string RequiredPermission { get; } = "mertoolbox.prefab";

        public override bool Execute(List<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out var player))
            {
                response = "You must be a player to use this command!";
                return false;
            }
            
            if (Enum.TryParse<ClutterType>(string.Join(" ", arguments), true, out var value))
            {
                GameObject clutterPrefab = UnityEngine.Object.Instantiate(ClutterManager.GetClutterPrefab(value));
                NetworkServer.UnSpawn(clutterPrefab);
                clutterPrefab.transform.rotation = player.Rotation;
                clutterPrefab.transform.position = player.Position;
                NetworkServer.Spawn(clutterPrefab);
                response = $"Spawned Prefab at {clutterPrefab.transform.position} rotation: {clutterPrefab.transform.rotation}";
                return true;
            }
            else
            {
                response = $"Incorrect Clutter Prefab! Available prefabs are: {string.Join(", ", Enum.GetNames(typeof(ClutterType)))}";
                return false;
            }
        }
    }
}