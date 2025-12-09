using System;
using System.Reflection;
using HarmonyLib;
using Interactables.Interobjects;
using MapGeneration;
using UnityEngine;

namespace MERToolbox.Patches
{
    [HarmonyPatch]
    public static class MERNukePatchFix
    {
        public static void UnpatchOriginal()
        {
            MethodInfo method = typeof(AlphaWarheadController).GetMethod( nameof(AlphaWarheadController.CanBeDetonated), BindingFlags.NonPublic | BindingFlags.Static, null, [typeof(Vector3), typeof(bool)], null);
            ProjectMER.ProjectMER.Singleton._harmony.Unpatch(method, HarmonyPatchType.Prefix, ProjectMER.ProjectMER.Singleton._harmony.Id);
        }

        [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CanBeDetonated))]
        public static class AlphaWarheadCanBeDetonatedFix
        {
            public static void Postfix(Vector3 pos, bool includeOnlyLifts, ref bool __result)
            {
                FacilityZone zone = pos.GetZone();
                if (zone is FacilityZone.Surface || zone is FacilityZone.None)
                {
                    __result = false;
                    return;
                }

                __result = !includeOnlyLifts || ElevatorChamber.AllChambers.Exists(ch => ch.WorldspaceBounds.Contains(pos)) || zone is FacilityZone.HeavyContainment || zone is FacilityZone.Entrance || zone is FacilityZone.LightContainment;
            }
        }
    }
}