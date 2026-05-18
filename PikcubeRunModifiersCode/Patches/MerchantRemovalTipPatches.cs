using System.Data;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

internal class MerchantRemovalTipPatches
{
    [HarmonyPatch(typeof(NMerchantCardRemoval), "Title", MethodType.Getter)]
    internal static class TitlePatch
    {
        [UsedImplicitly]
        internal static LocString Postfix(LocString __result, MerchantCardRemovalEntry ____removalEntry)
        {
            FieldInfo playerField = AccessTools.DeclaredField(typeof(MerchantEntry), "_player");
            Player p = (Player?)playerField.GetValue(____removalEntry) ?? throw new NoNullAllowedException();

            foreach (BlahajEnjoyer listener in p.RunState.IterateHookListeners(null)
                         .OfType<BlahajEnjoyer>())
            {
                listener.ModifyRemovalTitle(ref __result);
            }

            return __result;
        }
    }

    [HarmonyPatch(typeof(NMerchantCardRemoval), "Description", MethodType.Getter)]
    internal static class DescriptionPatch
    {
        [UsedImplicitly]
        internal static LocString Postfix(LocString __result, MerchantCardRemovalEntry ____removalEntry)
        {
            FieldInfo playerField = AccessTools.DeclaredField(typeof(MerchantEntry), "_player");
            Player p = (Player?)playerField.GetValue(____removalEntry) ?? throw new NoNullAllowedException();

            foreach (BlahajEnjoyer listener in p.RunState.IterateHookListeners(null)
                         .OfType<BlahajEnjoyer>())
            {
                listener.ModifyRemovalDescription(ref __result);
            }

            return __result;
        }
    }


}