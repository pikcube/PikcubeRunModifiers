using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class BlahajEnjoyer : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Bad;

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

    public void ModifyRemovalTitle(ref LocString title)
    {
        if (!RunState.Modifiers.Contains(this))
        {
            return;
        }
        title = new LocString("modifiers", "PIKCUBERUNMODIFIERS-BLAHAJ_ENJOYER.removalTitle");
    }

    public void ModifyRemovalDescription(ref LocString description)
    {
        if (!RunState.Modifiers.Contains(this))
        {
            return;
        }
        description = AscensionHelper.HasAscension(AscensionLevel.Inflation) ? 
            new LocString("modifiers", "PIKCUBERUNMODIFIERS-BLAHAJ_ENJOYER.removalDescriptionTwo") : 
            new LocString("modifiers", "PIKCUBERUNMODIFIERS-BLAHAJ_ENJOYER.removalDescription");
    }
}