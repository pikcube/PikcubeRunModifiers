using System.Runtime.InteropServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(OneOffSynchronizer), "DoMerchantCardRemoval")]
public static class ReplaceMerchantRemovalPatch
{
    public static bool Prefix(ref Task<bool> __result, Player player, int goldCost, bool cancelable = true)
    {
        if (!player.RunState.Modifiers.Any(m => m is BlahajEnjoyer))
        {
            return true;
        }

        __result = DoMerchantBottomSurgery(player, goldCost, cancelable);
        return false;

    }

    private static async Task<bool> DoMerchantBottomSurgery(Player player, int goldCost, bool cancelable = true)
    {
        CardModel? card = (await CardSelectCmd.FromDeckForTransformation(player, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1)
        {
            Cancelable = cancelable,
            RequireManualConfirmation = true
        })).FirstOrDefault();
        if (card != null)
        {
            await PlayerCmd.LoseGold(goldCost, player, GoldLossType.Spent);
            await CardCmd.Transform([new CardTransformation(card)], player.PlayerRng.Transformations);
            ++player.ExtraFields.CardShopRemovalsUsed;
        }
        bool flag = card != null;
        return flag;
    }
}