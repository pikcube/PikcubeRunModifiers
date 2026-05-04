using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class PerfectLethal() : PikcubeRunModifierModel(CustomRunType.Bad, "Perfect Lethal")
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
            DamageVar reflectedDamage = new(result.OverkillDamage, ValueProp.Unpowered | ValueProp.SkipHurtAnim);
            await CreatureCmd.Damage(choiceContext, dealer, reflectedDamage, null, null);
        }
    }
}