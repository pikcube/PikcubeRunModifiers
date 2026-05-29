using BaseLib.Abstracts;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class Neapolitan : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

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