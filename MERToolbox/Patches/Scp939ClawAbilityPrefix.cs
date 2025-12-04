using HarmonyLib;
using MERToolbox.API.Helpers;
using Mirror;
using PlayerRoles.PlayableScps.Scp939;

namespace MERToolbox.Patches
{
    [HarmonyPatch(typeof(Scp939ClawAbility))]
    public static class Scp939ClawAbilityPrefix
    {
        public static event System.Action<Scp939ClawAbility> OnClawAttempted;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Scp939ClawAbility.ServerProcessCmd))]
        public static void ServerProcessCmdPrefix(Scp939ClawAbility __instance, NetworkReader reader)
        {
            if (__instance is null)
                return;

            if (__instance._focusAbility.State != 0f)
                return;

            try
            {
                OnClawAttempted?.Invoke(__instance);
            }
            catch (System.Exception ex)
            {
                LogManager.Error(ex.Message);
            }
        }
    }
}