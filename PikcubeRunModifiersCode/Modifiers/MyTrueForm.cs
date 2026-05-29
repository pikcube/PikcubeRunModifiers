using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class MyTrueForm : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            CardPileCmd.Add(DemonForm.Create(p), PileType.Deck);
            CardPileCmd.Add(SerpentForm.Create(p), PileType.Deck);
            CardPileCmd.Add(VoidForm.Create(p), PileType.Deck);
            CardPileCmd.Add(ReaperForm.Create(p), PileType.Deck);
            CardPileCmd.Add(EchoForm.Create(p), PileType.Deck);
        }
    }
}