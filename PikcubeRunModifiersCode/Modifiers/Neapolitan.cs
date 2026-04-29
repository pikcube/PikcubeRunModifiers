using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class Neapolitan : PikcubeModifier
{
    static Neapolitan()
    {
        new RelicSpawnManager().RegisterRule<IceCream>(Predicates.UnlessModifierPresent<Neapolitan>);
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            RelicCmd.Obtain<IceCream>(p);
        }
    }
}