using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class Neapolitan : PikcubeModifier
{
    static Neapolitan()
    {
        MainFile.RelicSpawnManager.RegisterRule<IceCream>(runstate => !runstate.Modifiers.Any(m => m is Neapolitan));
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            RelicCmd.Obtain<IceCream>(p);
        }
    }
}