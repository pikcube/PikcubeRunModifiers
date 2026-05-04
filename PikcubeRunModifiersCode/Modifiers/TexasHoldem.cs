using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class TexasHoldem() : PikcubeRunModifierModel(CustomRunType.Good, "Texas Holdem")
{
    static TexasHoldem()
    {
        new RelicSpawnManager().RegisterRule<RunicPyramid>(Predicates.UnlessModifierPresent<TexasHoldem>);
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is RunicPyramid))
            {
                continue;
            }
            RelicCmd.Obtain<RunicPyramid>(p);
        }
    }
}