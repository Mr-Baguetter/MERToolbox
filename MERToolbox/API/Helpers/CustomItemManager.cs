using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Misc;
using MERToolbox.API.Data;
using MERToolbox.API.Enums;
using ProjectMER.Features.Objects;
using UnityEngine;

namespace MERToolbox.API.Helpers
{
    public class CustomItemManager
    {
        public static bool ExiledInstalled { get; set; }
        public static bool UCIInstalled { get; set; }
        public static bool LabApiCIInstalled { get; set; }

        public static Assembly UCIAssembly { get; set; }
        public static Assembly LabApiAssembly { get; set; }

        public static Type ExiledPickupType { get; set; }
        public static Type ExiledCustomItemType { get; set; }
        public static Type SummonedCustomItem { get; set; }
        public static Type Utilities { get; set; }

        public static MethodInfo ExiledSpawn { get; set; }
        public static MethodInfo LabApiSpawn { get; set; }

        public static void Init()
        {
            foreach (LabApi.Loader.Features.Plugins.Plugin plugin in LabApi.Loader.PluginLoader.EnabledPlugins)
            {
                if (plugin.Name is "CustomItemsAPI")
                {
                    plugin.TryGetLoadedAssembly(out Assembly assembly);
                    LabApiAssembly = assembly;
                    LabApiCIInstalled = true;
                }

                if (plugin.Name is "UncomplicatedCustomItems")
                {
                    plugin.TryGetLoadedAssembly(out Assembly assembly);
                    UCIAssembly = assembly;
                    UCIInstalled = true;
                }
            }

            ExiledCustomItemType = FindTypeInLoadedAssemblies("Exiled.CustomItems.API.Features.CustomItem");
            if (ExiledCustomItemType != null)
                ExiledInstalled = true;

            if (UCIInstalled)
            {
                LogManager.Debug($"UCI Found!");
                SummonedCustomItem = UCIAssembly?.GetType("UncomplicatedCustomItems.API.Features.SummonedCustomItem");
                Utilities = UCIAssembly?.GetType("UncomplicatedCustomItems.API.Utilities");
            }

            if (LabApiCIInstalled)
            {
                LogManager.Debug($"LabApi CustomItems API Found!");
                Type customItemType = LabApiAssembly.GetType("CustomItemsAPI.CustomItems");
                if (customItemType is not null)
                {
                    Type[] parameters = [typeof(string), typeof(Vector3), typeof(Quaternion), typeof(Vector3), typeof(bool)];
                    LabApiSpawn = customItemType.GetMethod("Spawn", BindingFlags.Static | BindingFlags.Public, null, parameters, null);
                }
            }

            if (ExiledInstalled)
            {
                LogManager.Debug($"Exiled Found!");
                ExiledSpawn = GetExiledSpawnMethod();   
            }
        }

        public static bool TrySpawnItems(SchematicObject schematic)
        {
            foreach (CustomItemSpawn itemSpawn in ConfigManager.CustomItemSpawns)
            {
                if (schematic.Name == itemSpawn.FileName)
                {
                    ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, itemSpawn.Position, itemSpawn.Rotation, out Vector3 position, out Quaternion rotation);
                    switch (itemSpawn.CustomItemType)
                    {
                        case CustomItemType.ExiledCustomItem:
                            LogManager.Debug($"Spawning Exiled CustomItem");
                            ExiledSpawnCustomItem(uint.Parse(itemSpawn.Id), position, rotation);
                            break;
                        
                        case CustomItemType.LabApiCustomItem:
                            LogManager.Debug($"Spawning LabApi CustomItem");
                            LabApiSpawnCustomItem(itemSpawn.Id, position, rotation);
                            break;

                        case CustomItemType.UCICustomItem:
                            LogManager.Debug($"Spawning UCI CustomItem");
                            UCISpawnCustomItem(uint.Parse(itemSpawn.Id), position, rotation);
                            break;
                    }
                }
            }

            return true;
        }

        public static void DestoryItems(SchematicObject schematic)
        {
            foreach (CustomItemSpawn itemSpawn in ConfigManager.CustomItemSpawns.ToArray())
            {
                if (schematic.Name == itemSpawn.FileName)
                {
                    foreach (Pickup item in Pickup.List.ToArray())
                    {
                        ConfigManager.CalculateWorldTransform(schematic.Position, schematic.Rotation, itemSpawn.Position, itemSpawn.Rotation, out Vector3 position, out _);
                        if (item.Position == position)
                            item.Destroy();
                    }
                }
            }
        }

        private static MethodInfo GetExiledSpawnMethod()
        {
            ExiledPickupType = Type.GetType("Exiled.API.Features.Pickups.Pickup, Exiled.API");
            if (ExiledPickupType == null)
            {
                LogManager.Error($"Could not load Exiled pickup type: Exiled.API.Features.Pickups.Pickup, Exiled.API");
                return null;
            }

            Type[] parameterTypes = [typeof(uint), typeof(Vector3), ExiledPickupType.MakeByRefType()];
            return ExiledCustomItemType.GetMethod("TrySpawn", BindingFlags.Static | BindingFlags.Public, null, parameterTypes, null);
        }

        private static bool UCIHasCustomItem(uint id, out object customItem)
        {
            customItem = null;

            if (!UCIInstalled)
                return false;

            LogManager.Debug($"UCI found, checking if the item {id} exists...");
            try
            {
                MethodInfo hasCustomItem = Utilities.GetMethod("IsCustomItem", BindingFlags.Public | BindingFlags.Static);
                MethodInfo getCustomItem = Utilities.GetMethod("GetCustomItem", BindingFlags.Public | BindingFlags.Static);

                if (hasCustomItem is not null && getCustomItem is not null)
                {
                    if ((bool)hasCustomItem.Invoke(null, [id]))
                    {
                        customItem = getCustomItem.Invoke(null, [id]);
                        return customItem is not null;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                LogManager.Error(e.ToString());
                return false;
            }
        }

        public static void UCISpawnCustomItem(uint id, Vector3 pos, Quaternion rot)
        {
            if (!UCIInstalled)
                return;

            LogManager.Debug($"UCI found, trying to spawn item {id}");

            try
            {
                if (UCIHasCustomItem(id, out object customItem) && customItem is not null)
                {
                    SummonedCustomItem.GetConstructor([UCIAssembly.GetType("UncomplicatedCustomItems.API.Interfaces.ICustomItem"), typeof(Vector3), typeof(Quaternion)]).Invoke([customItem, pos, rot]);
                    LogManager.Debug($"Spawned UCI CustomItem");
                }
            }
            catch (Exception e)
            {
                LogManager.Error(e.ToString());
            }
        }

        public static void LabApiSpawnCustomItem(string name, Vector3 pos, Quaternion rot)
        {
            if (LabApiSpawn is null || !LabApiCIInstalled)
                return;

            LabApiSpawn.Invoke(null, [name, pos, rot, null, null]);
            LogManager.Debug($"Spawned LabApi CustomItem");
        }

        public static void ExiledSpawnCustomItem(uint id, Vector3 pos, Quaternion rot)
        {
            if (ExiledSpawn == null || !ExiledInstalled)
            {
                LogManager.Error("Exiled is not installed or spawn method is null");
                return;
            }

            try
            {
                object[] parameters = [id, pos, null];
                bool success = (bool)ExiledSpawn.Invoke(null, parameters);
                
                if (success && parameters[2] != null)
                {
                    object pickupObj = parameters[2];
                    Type pickupType = pickupObj.GetType();
                    PropertyInfo rotationProp = pickupType.GetProperty("Rotation", BindingFlags.Public | BindingFlags.Instance);
                    if (rotationProp != null)
                    {
                        rotationProp.SetValue(pickupObj, rot);
                        LogManager.Debug($"Spawned Exiled CustomItem with rotation");
                    }
                    else
                        LogManager.Warn("Could not find Rotation property on pickup");
                }
                else
                    LogManager.Warn($"Failed to spawn Exiled CustomItem with ID: {id}");
            }
            catch (Exception e)
            {
                LogManager.Error($"Error spawning Exiled CustomItem: {e}");
            }
        }

        /// <summary>
        /// Searches all loaded assemblies for a type with the specified full name.
        /// </summary>
        /// <param name="fullTypeName">The full name of the type to find.</param>
        /// <returns>The Type if found; null otherwise.</returns>
        private static Type FindTypeInLoadedAssemblies(string fullTypeName) => AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetTypesFromAssembly).FirstOrDefault(t => t.FullName == fullTypeName);

        /// <summary>
        /// Safely retrieves all types from an assembly.
        /// </summary>
        /// <param name="assembly">The assembly to get types from.</param>
        /// <returns>Array of types from the assembly.</returns>
        private static Type[] GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).ToArray();
            }
            catch (Exception ex)
            {
                LogManager.Debug($"Failed to load types from assembly '{assembly.FullName}': {ex.Message}");
                return [];
            }
        }
    }
}