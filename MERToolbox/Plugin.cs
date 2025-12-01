global using Logger = LabApi.Features.Console.Logger;
using HarmonyLib;
using LabApi.Loader.Features.Plugins;
using MERToolbox.API;
using System;

namespace MERToolbox
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "MERToolbox";
        public override string Description => ":3";
        public override string Author => "Mr. Baguetter";
        public override Version RequiredApiVersion => LabApi.Features.LabApiProperties.CurrentVersion;
        public override Version Version => new(1, 0, 0, 8);
        public static Plugin Instance;
        internal AudioApi AudioApi;
        internal Harmony _harmony;

        public override void Enable()
        {
            Instance = this;
            AudioApi = new();
            _harmony = new($"MrBaguetter-MERToolbox-{DateTime.Now}");
            _harmony.PatchAll();

            AudioApi.SetSoundLists(Config.AudioPathing);
            MERHandler.Register();
            Handler.Register();
        }

        public override void Disable()
        {
            MERHandler.Unregister();
            Handler.Unregister();
            _harmony.UnpatchAll();

            _harmony = null;
            AudioApi = null;
            Instance = null;
        }
    }
}
