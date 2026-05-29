using BaseLib.Abstracts;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Enchantments;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class StickyStartersModifier : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Bad;
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            foreach (CardModel card in p.Deck.Cards.Where(c => c.Rarity == CardRarity.Basic))
            {
                CardCmd.Enchant<Sticky>(card, 1);
            }
        }
    }
}