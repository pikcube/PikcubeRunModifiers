using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

//+1 Energy, -1 Card per turn
public class FourForFourModifier() : PikcubeRunModifierModel(CustomRunType.Bad, "4 for 4")
{
    public override bool ShouldReceiveCombatHooks => true;

    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player player in runState.Players)
        {
            ++player.MaxEnergy;
        }
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        return count - 1;
    }
}