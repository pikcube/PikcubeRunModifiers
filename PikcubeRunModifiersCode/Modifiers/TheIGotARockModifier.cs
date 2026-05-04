using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class TheIGotARockModifier() : PikcubeRunModifierModel(CustomRunType.Bad, "I Got a Rock")
{
    static TheIGotARockModifier()
    {
        new RelicSpawnManager().RegisterRule<PetrifiedToad>(Predicates.UnlessModifierPresent<TheIGotARockModifier>);
    }

    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is PetrifiedToad))
            {
                continue;
            }

            RelicCmd.Obtain<PetrifiedToad>(p);
        }
    }

    public override async Task AfterPotionProcured(PotionModel potion)
    {
        foreach (PotionModel p in potion.Owner.Potions.ToArray())
        {
            if (p is PotionShapedRock)
            {
                continue;
            }

            await PotionCmd.Discard(p);
            await PotionCmd.TryToProcure<PotionShapedRock>(p.Owner);
        }
    }

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        bool isModified = false;
        foreach (Reward reward in rewards.ToArray())
        {
            if (reward is not PotionReward pw)
            {
                continue;
            }

            int index = rewards.IndexOf(reward);
            rewards[index] = new PotionReward((PotionShapedRock)ModelDb.Potion<PotionShapedRock>().ToMutable(), player);
            isModified = true;
        }

        return isModified;
    }
}