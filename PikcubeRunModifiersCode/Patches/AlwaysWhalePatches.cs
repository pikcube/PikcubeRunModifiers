using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using Pikcube.Common.Utility;
using System.Reflection.Emit;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

public static class AlwaysWhalePatches
{
    static AlwaysWhalePatches()
    {
        BetterHooks.AfterRunInitialized += BetterHooks_AfterRunInitialized;
    }

    private static void BetterHooks_AfterRunInitialized(MegaCrit.Sts2.Core.Runs.RunState runState)
    {
        ModifyGenerateInitialOptions = null;
    }

    public static event EventHandler<ModifyInitialArgs>? ModifyGenerateInitialOptions;

    public class ModifyInitialArgs(IReadOnlyList<EventOption> original, Neow neow)
    {
        public IReadOnlyList<EventOption> NewList { get; set; } = original;
        public Neow Neow { get; set; } = neow;
    }

    [HarmonyPatch(typeof(Neow), "GenerateInitialOptions", MethodType.Normal)]
    public static class TriggerRunModifierPatch
    {
        public static IReadOnlyList<EventOption> Postfix(IReadOnlyList<EventOption> __result, Neow __instance)
        {
            ModifyInitialArgs args = new(__result, __instance);
            ModifyGenerateInitialOptions?.Invoke(__instance, args);
            return args.NewList;
        }
    }


    [HarmonyPatch]
    public static class NeowReverseOptionsPatch
    {
        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Neow), "GenerateInitialOptions")]
        public static IReadOnlyList<EventOption> GenerateInitialOptionsWithoutModifiers(object instance)
        {
            _ = instance.GetType();
            _ = Transpiler(null!);
            return [];

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                CodeMatcher matcher = new(instructions);

                matcher.MatchStartForward(CodeMatch.WithOpcodes([OpCodes.Bgt]));
                matcher.ThrowIfInvalid("Could not find branch instruction");
                object? operand = matcher.Instruction.operand;
                matcher.RemoveInstruction();
                matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Blt, operand));

                return matcher.Instructions();
            }
        }
    }
}