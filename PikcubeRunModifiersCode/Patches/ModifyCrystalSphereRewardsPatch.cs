using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(OneOffSynchronizer), "OfferCrystalSphereRewards")]
public class ModifyCrystalSphereRewardsPatch
{
    public delegate void ModifyCrystalSphereRewardsHandler(ref List<Reward> rewards, Player owner);

    public static event ModifyCrystalSphereRewardsHandler? ModifyCrystalSphereRewards;

    public static bool Prefix(ref Task __result, Player owner, List<CrystalSphereItem> revealed, Rng rng)
    {
        if (!owner.RunState.Modifiers.Any(m => m is FortuneFavorsTheBold))
        {
            return true;
        }
        __result = OfferModifiedCrystalSphereRewards(owner, revealed, rng);
        return false;

    }

    private static async Task OfferModifiedCrystalSphereRewards(Player owner, List<CrystalSphereItem> revealed, Rng rng)
    {
        List<Reward> list = [.. revealed.Select((Func<CrystalSphereItem, Reward?>)(r => r.ToReward(owner, rng))).OfType<Reward>()];

        ModifyCrystalSphereRewards?.Invoke(ref list, owner);

        if (owner.RunState.CurrentRoom is CombatRoom { RoomType: RoomType.Boss } ||
            owner.RunState.CurrentRoom?.IsVictoryRoom is true)
        {
            if (owner.RunState.Map.SecondBossMapPoint == null)
            {
                await ShowTerminalBossReward(owner, list);
                return;
            }

            MapCoord? currentMapCoord = owner.RunState.CurrentMapCoord;
            MapCoord coord = owner.RunState.Map.BossMapPoint.coord;
            if (currentMapCoord.HasValue && currentMapCoord.GetValueOrDefault() == coord)
            {
                await ShowTerminalBossReward(owner, list);
                return;
            }
        }

        await RewardsCmd.OfferCustom(owner, list);
    }

    private static async Task ShowTerminalBossReward(Player owner, List<Reward> list)
    {
        RewardsSet rewardsSet = new(owner);
        rewardsSet.WithCustomRewards(list);
        PropertyInfo? propertyInfo = AccessTools.DeclaredProperty(typeof(RewardsSet), nameof(RewardsSet.Room));
        if (propertyInfo is null)
        {
            return;
        }
        propertyInfo.SetValue(rewardsSet, owner.RunState.CurrentRoom);
        await rewardsSet.Offer();
    }
}