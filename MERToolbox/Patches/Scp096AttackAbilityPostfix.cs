using HarmonyLib;
using MERToolbox.API.Helpers;
using PlayerRoles.PlayableScps.Scp096;

namespace MERToolbox.Patches
{
    
    [HarmonyPatch(typeof(Scp096AttackAbility))]
    public static class Scp096AttackAbilityPostfix
    {
        public static event System.Action<Scp096AttackAbility> OnSwingTriggered;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Scp096AttackAbility.OnKeyDown))]
        public static void OnKeyDownPostfix(Scp096AttackAbility __instance)
        {
            if (__instance is null)
                return;

            if (!__instance.AttackPossible || !__instance._clientAttackCooldown.IsReady)
                return;

            try
            {
                OnSwingTriggered?.Invoke(__instance);
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex.Message);
            }
        }
    }

}