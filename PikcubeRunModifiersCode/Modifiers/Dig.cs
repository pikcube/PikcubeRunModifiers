using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class Dig : PikcubeModifier
{
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