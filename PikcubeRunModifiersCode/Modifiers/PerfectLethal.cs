using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class PerfectLethal : PikcubeModifier
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result,
        ValueProp props,
        Creature target, CardModel? cardSource)
    {
        if (!result.WasTargetKilled)
        {
            return;
        }
        if (result.OverkillDamage == 0)
        {
            return;
        }
        if (dealer is null)
        {
            return;
        }
        if (props.IsPoweredAttack() || cardSource is Omnislice)
        {
            await CreatureCmd.Damage(choiceContext, dealer, new DamageVar(result.OverkillDamage, ValueProp.Unpowered | ValueProp.SkipHurtAnim), null, null);
        }
    }
}