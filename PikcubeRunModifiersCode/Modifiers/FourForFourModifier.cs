using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

//+1 Energy, -1 Card per turn
public class FourForFourModifier : PikcubeModifier
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