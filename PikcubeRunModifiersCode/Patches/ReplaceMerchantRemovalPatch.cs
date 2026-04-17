using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(OneOffSynchronizer), "DoMerchantCardRemoval")]
public static class ReplaceMerchantRemovalPatch
{
    static ReplaceMerchantRemovalPatch()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
    }

    private static void BetterHooks_AfterRunInitialized(MegaCrit.Sts2.Core.Runs.RunState runState)
    {
        ModifyMerchantCardRemoval = null;
    }

    [UsedImplicitly]
    public static bool Prefix(ref Task<bool> __result, Player player, ref int goldCost, ref bool cancelable)
    {
        ModifyMerchantRemovalArgs args = new(__result, player, goldCost, cancelable);
        ModifyMerchantCardRemoval?.Invoke(null, args);
        goldCost = args.GoldCost;
        cancelable = args.Cancelable;
        __result = args.NewMerchantRemoval;
        return args.OriginalMerchantRemoval == args.NewMerchantRemoval;
    }

    public static event EventHandler<ModifyMerchantRemovalArgs>? ModifyMerchantCardRemoval;

}

public class ModifyMerchantRemovalArgs(Task<bool> originalMerchantRemoval, Player player, int goldCost, bool cancelable)
{
    public Task<bool> OriginalMerchantRemoval { get; } = originalMerchantRemoval;
    public Task<bool> NewMerchantRemoval { get; set; } = originalMerchantRemoval;
    public Player Player { get; } = player;

    public int GoldCost { get; set; } = goldCost;
    public bool Cancelable { get; set; } = cancelable;

}