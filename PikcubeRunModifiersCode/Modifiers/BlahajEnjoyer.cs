using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class BlahajEnjoyer() : PikcubeRunModifierModel(CustomRunType.Bad, "Blahaj Enjoyer")
{ 
    protected override void AfterRunCreated(RunState runState)
    {
        ReplaceMerchantRemovalPatch.ModifyMerchantCardRemoval += ReplaceMerchantRemovalPatch_ModifyMerchantCardRemoval;
    }

    protected override void AfterRunLoaded(RunState runState)
    {
        ReplaceMerchantRemovalPatch.ModifyMerchantCardRemoval += ReplaceMerchantRemovalPatch_ModifyMerchantCardRemoval;
    }

    private void ReplaceMerchantRemovalPatch_ModifyMerchantCardRemoval(object? sender, ModifyMerchantRemovalArgs e)
    {
        e.NewMerchantRemoval = DoMerchantBottomSurgery(e.Player, e.GoldCost, e.Cancelable);
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