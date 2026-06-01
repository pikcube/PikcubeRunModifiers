using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
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

        await RewardsCmd.OfferCustom(owner, list);
    }
}