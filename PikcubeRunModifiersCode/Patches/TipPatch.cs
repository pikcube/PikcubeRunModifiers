using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Relics;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

[HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
public static class TipPatch
{
    [UsedImplicitly]
    public static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
    {
        return !RunManager.Instance.IsInProgress || __instance.IsCanonical || __instance.Owner?.Relics.Any(r => r is Gadget) is true ? __result.Append(new HoverTip(Gadget.Canonical.Title, Gadget.GetGadgetTip(__instance))) : __result;
    }
}