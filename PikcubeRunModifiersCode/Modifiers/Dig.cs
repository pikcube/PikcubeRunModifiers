using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class Dig : PikcubeModifier
{
    static Dig()
    {
        new RelicSpawnManager().RegisterRule<Shovel>(Predicates.UnlessModifierPresent<Dig>);
    }
    
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is Shovel))
            {
                continue;
            }
            RelicCmd.Obtain<Shovel>(p);
        }
    }
}