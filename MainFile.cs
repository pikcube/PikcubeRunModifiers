using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace PikcubeRunModifiers
{
    [ModInitializer(nameof(Initialize))]
    public partial class MainFile : Node
    {
        public const string ModId = "PikcubeRunModifiers"; //Used for resource filepath

        public static Logger Logger { get; } = new(ModId, LogType.Generic);

        public static void Initialize()
        {
            Harmony harmony = new(ModId);

            harmony.PatchAll();

            BetterHooks.AfterOneTimeInitialization += BetterHooks_AfterOneTimeInitialization;
        }

        private static void BetterHooks_AfterOneTimeInitialization()
        {
            _ = PikcubeRunModifierModel.ModifierMap;
        }
    }
}
