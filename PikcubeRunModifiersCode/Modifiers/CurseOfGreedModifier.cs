using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Powers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class CurseOfGreedModifier : PikcubeModifier
{
    static CurseOfGreedModifier()
    {
        MainFile.RelicSpawnManager.RegisterRule<MoltenEgg>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<ToxicEgg>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<FrozenEgg>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<Whetstone>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<WarPaint>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<StoneCracker>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<RazorTooth>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<Bellows>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<Pomander>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
        MainFile.RelicSpawnManager.RegisterRule<LavaLamp>(runState => !runState.Modifiers.Any(m => m is CurseOfGreedModifier));
    }

    public override bool ShouldReceiveCombatHooks => true;

    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            CardCmd.Upgrade(p.Deck.Cards, CardPreviewStyle.None);
        }
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Apply<Cursed>(player.Creature, 1, null, null);
    }

    public override bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewards, CardCreationOptions options)
    {
        if (options.Flags.HasFlag(CardCreationFlags.NoHookUpgrades))
        {
            return false;
        }

        UpgradeValidCards(cardRewards, _ => true);

        return true;
    }

    public override void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)
    {
        UpgradeValidCards(cards, _ => true);
    }

    public override bool TryModifyCardBeingAddedToDeck(CardModel card, out CardModel? newCard)
    {
        if (!card.IsUpgradable || card.CurrentUpgradeLevel >= 1)
        {
            newCard = null;
            return false;
        }

        newCard = card.Owner.RunState.CloneCard(card);
        CardCmd.Upgrade(newCard, CardPreviewStyle.None);
        return true;
    }

    private static void UpgradeValidCards(IEnumerable<CardCreationResult> cards, Predicate<CardModel> filter)
    {
        foreach (CardCreationResult cardCreationResult in cards.Where(c => c.Card.IsUpgradable && filter(c.Card)))
        {
            CardModel card = cardCreationResult.Card.Owner.RunState.CloneCard(cardCreationResult.Card);
            CardCmd.Upgrade(card);
            cardCreationResult.ModifyCard(card);
        }
    }
}