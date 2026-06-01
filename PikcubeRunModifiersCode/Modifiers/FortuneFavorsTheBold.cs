using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class FortuneFavorsTheBold : PikcubeRunModifierModel
{
    public bool? IsReady { get; set; }

    public override ModifierAlignment Alignment => ModifierAlignment.Good;
    public override Task BeforeCombatStart()
    {
        IsReady = true;
        return Task.CompletedTask;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (IsReady is false)
        {
            return false;
        }

        if (room is CombatRoom)
        {
            IsReady = true;
        }
        else
        {
            return false;
        }

        rewards.RemoveAll(r => r is CardReward);

        List<Reward> original = [.. rewards];

        rewards.Clear();

        if (player.Gold > 50)
        {
            int price = player.PlayerRng.Rewards.NextInt(51, 100);
            if (player.Gold < price)
            {
                price = player.Gold;
            }
            rewards.Add(new MinigameReward(player, original, price));
        }

        rewards.Add(new MinigameReward(player, original, -1));

        return true;
    }

    public override Task AfterModifyingRewards()
    {
        IsReady = false;
        return Task.CompletedTask;
    }
}