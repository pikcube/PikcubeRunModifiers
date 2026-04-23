using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class Inception : PikcubeModifier
{
    static Inception()
    {
        MainFile.RelicSpawnManager.RegisterRule<UnceasingTop>(Predicates.UnlessModifierPresent<Inception>);
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            RelicCmd.Obtain<UnceasingTop>(p);
        }
    }
}