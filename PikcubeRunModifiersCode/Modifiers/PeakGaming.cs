using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class PeakGaming() : PikcubeRunModifierModel(CustomRunType.Bad, "Peak Gaming")
{
    static PeakGaming()
    {
        new EventSpawnManager().RegisterRule<DollRoom>(Predicates.UnlessModifierPresent<PeakGaming>);
    }

    public bool IsNopeModeEnabled { get; set; } = false;
    private readonly List<CardModel> _cards = [];

    public override bool ShouldAddToDeck(CardModel card)
    {
        return !IsNopeModeEnabled || _cards.Contains(card);
    }

    protected override void AfterRunCreated(RunState runState)
    {
        IsNopeModeEnabled = true;
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is BingBong))
            {
                continue;
            }

            CardPileCmd.Add(p.Deck.Cards.Select(c =>
            {
                CardModel newCard = p.RunState.CloneCard(c);
                _cards.Add(newCard);
                return newCard;
            }), PileType.Deck, skipVisuals: true);
            RelicCmd.Obtain<BingBong>(p);
        }

        IsNopeModeEnabled = false;
    }
}