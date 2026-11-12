using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Code.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(MerchantCardRemovalEntry), nameof(MerchantCardRemovalEntry.DoCardRemoval))]
public static class ReplaceMerchantRemovalPatch
{
    static ReplaceMerchantRemovalPatch()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
    }

    private static void BetterHooks_AfterRunInitialized(RunState runState)
    {
        ModifyMerchantCardRemoval = null;
    }

    [UsedImplicitly]
    public static bool Prefix(ref Task<bool> __result, Player player, ref bool isCancelable)
    {
        ModifyMerchantRemovalArgs args = new(__result, player, isCancelable);
        ModifyMerchantCardRemoval?.Invoke(null, args);
        isCancelable = args.Cancelable;
        __result = args.NewMerchantRemoval;
        return args.OriginalMerchantRemoval == args.NewMerchantRemoval;
    }

    public static event EventHandler<ModifyMerchantRemovalArgs>? ModifyMerchantCardRemoval;

}

public class ModifyMerchantRemovalArgs(Task<bool> originalMerchantRemoval, Player player, bool cancelable)
{
    public Task<bool> OriginalMerchantRemoval { get; } = originalMerchantRemoval;
    public Task<bool> NewMerchantRemoval { get; set; } = originalMerchantRemoval;
    public Player Player { get; } = player;
    public bool Cancelable { get; set; } = cancelable;

}