using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class PraiseSnecko : PikcubeModifier
{
    static PraiseSnecko()
    {
        new RelicSpawnManager().RegisterRule<SneckoEye>(Predicates.UnlessModifierPresent<PraiseSnecko>);
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is SneckoEye))
            {
                continue;
            }
            RelicCmd.Obtain<SneckoEye>(p);
        }
    }
}