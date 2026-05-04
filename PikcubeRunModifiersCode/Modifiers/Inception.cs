using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class Inception() : PikcubeRunModifierModel(CustomRunType.Good, nameof(Inception))
{
    static Inception()
    {
        new RelicSpawnManager().RegisterRule<UnceasingTop>(Predicates.UnlessModifierPresent<Inception>);
    }
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            RelicCmd.Obtain<UnceasingTop>(p);
        }
    }
}