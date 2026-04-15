using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class PerfectLethal : PikcubeModifier
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result,
        ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (result is { WasTargetKilled: true, OverkillDamage: > 0 } && dealer is not null)
        {
            await CreatureCmd.Damage(choiceContext, dealer, new DamageVar(result.OverkillDamage, ValueProp.Unpowered), null, null);
        }
    }
}