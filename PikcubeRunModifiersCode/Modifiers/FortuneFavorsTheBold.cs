using System.Reflection;
using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class FortuneFavorsTheBold : PikcubeRunModifierModel
{
    static FortuneFavorsTheBold()
    {
        RewardScreenPatches.OnRewardScreenShown += RewardScreenPatches_OnRewardScreenShown;
    }

    private static void RewardScreenPatches_OnRewardScreenShown(NRewardsScreen screen, RewardsSet set)
    {
        RewardMap.Add(set.Rewards, set);
        ScreenMap.Add(set.Rewards, screen);
    }

    public bool? IsReady { get; set; }

    private static ConditionalWeakTable<List<Reward>, RewardsSet> RewardMap { get; } = [];
    private static ConditionalWeakTable<List<Reward>, NRewardsScreen> ScreenMap { get; } = [];

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

        if (room is CombatRoom cr)
        {
            if (player.RunState.CurrentActIndex == 2 && cr.RoomType == RoomType.Boss)
            {
                return false;
            }
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

        rewards.Add(new NopeReward(player, original, rewards));

        return true;
    }

    public override Task AfterModifyingRewards()
    {
        IsReady = false;
        return Task.CompletedTask;
    }

    public static async Task ModifyAsync(Player player, List<Reward> original, List<Reward> current)
    {
        if (!ScreenMap.TryGetValue(current, out NRewardsScreen? screen))
        {
            return;
        }

        NOverlayStack.Instance?.Remove(screen);
        RewardsSet rewardsSet = new(player);
        rewardsSet.WithCustomRewards(original);
        PropertyInfo? propertyInfo = AccessTools.DeclaredProperty(typeof(RewardsSet), nameof(RewardsSet.Room));
        if (propertyInfo is null)
        {
            return;
        }
        propertyInfo.SetValue(rewardsSet, player.RunState.CurrentRoom);
        await rewardsSet.Offer();
    }
}