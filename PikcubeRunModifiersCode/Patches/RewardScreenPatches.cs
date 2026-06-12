using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Rewards;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(NRewardsScreen), nameof(NRewardsScreen.ShowScreen))]
public static class RewardScreenPatches
{
    public static void Postfix(NRewardsScreen __result, RewardsSet set)
    {
        OnRewardScreenShown?.Invoke(__result, set);
    }

    public delegate void OnRewardScreenShownHandler(NRewardsScreen screen, RewardsSet set);

    public static event OnRewardScreenShownHandler? OnRewardScreenShown;
}