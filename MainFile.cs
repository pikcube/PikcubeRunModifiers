using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace PikcubeRunModifiers;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "PikcubeRunModifiers"; //Used for resource filepath

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();

        ModConfigRegistry.Register(ModId, new MapConfigMenu());

        BetterHooks.AfterOneTimeInitialization += BetterHooks_AfterOneTimeInitialization;
    }

    private static void BetterHooks_AfterOneTimeInitialization()
    {
        _ = PikcubeRunModifierModel.ModifierMap;
    }
}