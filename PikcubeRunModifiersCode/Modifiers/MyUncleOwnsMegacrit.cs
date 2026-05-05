using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class MyUncleOwnsMegacrit() : PikcubeRunModifierModel(CustomRunType.Good, "My Uncle Owns Megacrit")
{
    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        foreach (CardReward cardReward in rewards.OfType<CardReward>().ToArray())
        {
            rewards.TryReplaceValue(cardReward,
                new CardReward(CardCreationOptions.ForRoom(player, RoomType.Boss), 3, player));
        }

        return true;
    }
}