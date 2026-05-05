using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class MyTrueForm() : PikcubeRunModifierModel(CustomRunType.Good, "My True Form")
{
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            CardPileCmd.Add(DemonForm.CreateInstance(p), PileType.Deck);
            CardPileCmd.Add(SerpentForm.CreateInstance(p), PileType.Deck);
            CardPileCmd.Add(VoidForm.CreateInstance(p), PileType.Deck);
            CardPileCmd.Add(ReaperForm.CreateInstance(p), PileType.Deck);
            CardPileCmd.Add(EchoForm.CreateInstance(p), PileType.Deck);
        }
    }
}