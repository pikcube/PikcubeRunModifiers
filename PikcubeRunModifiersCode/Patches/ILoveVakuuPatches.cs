using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rooms;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;


public static class LoveVakuuPatches
{
    static LoveVakuuPatches()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
    }

    private static void BetterHooks_AfterRunInitialized(MegaCrit.Sts2.Core.Runs.RunState runState)
    {
        ModifyAncientForAct = null;
        ModifyGenerateInitialOptions = null;
    }

    [HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
    public static class AncientSpawnPatch
    {
        public static void Postfix(ActModel __instance)
        {
            RoomSet? rooms = Traverse.Create(__instance).Field<RoomSet>("_rooms").Value;
            if (!rooms.HasAncient)
            {
                return;
            }
            AncientEventModel rngChosenAncient = rooms.Ancient;

            AncientEventArgs ancientEventArgs = new(rngChosenAncient);
            ModifyAncientForAct?.Invoke(__instance, ancientEventArgs);
            rooms.Ancient = ancientEventArgs.NewAncient;
        }
    }

    [HarmonyPatch(typeof(Vakuu), "GenerateInitialOptions", MethodType.Normal)]
    public static class TriggerRunModifierPatch
    {
        public static IReadOnlyList<EventOption> Postfix(IReadOnlyList<EventOption> __result, Vakuu __instance)
        {
            ModifyInitialArgs args = new(__result, __instance);
            ModifyGenerateInitialOptions?.Invoke(__instance, args);
            return args.NewList;
        }
    }

    public static event EventHandler<AncientEventArgs>? ModifyAncientForAct;
    public class AncientEventArgs(AncientEventModel newAncient)
    {
        public AncientEventModel OriginalAncient { get; } = newAncient;
        public AncientEventModel NewAncient { get; set; } = newAncient;
    }

    public static event EventHandler<ModifyInitialArgs>? ModifyGenerateInitialOptions;

    public class ModifyInitialArgs(IReadOnlyList<EventOption> original, Vakuu vakuu)
    {
        public IReadOnlyList<EventOption> NewList { get; set; } = original;
        public Vakuu Vakuu { get; set; } = vakuu;
    }


}