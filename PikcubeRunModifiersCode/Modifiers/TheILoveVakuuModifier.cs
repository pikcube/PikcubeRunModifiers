using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;
using System.Reflection;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class TheILoveVakuuModifier() : PikcubeRunModifierModel(CustomRunType.Good, "I Love Vakuu")
{
    private Dictionary<ActModel, RunState> ActsToModify { get; } = [];
    private List<EventOption> ModifierOptions { get; } = [];
    private Dictionary<ulong, IReadOnlyList<EventOption>> OriginalVakuuOptions { get; set; } = [];

    protected override void AfterRunCreated(RunState runState)
    {
        ActsToModify.Clear();
        ModifierOptions.Clear();
        foreach (ActModel act in RunState.Acts)
        {
            ActsToModify.Add(act, runState);
        }
        LoveVakuuPatches.ModifyAncientForAct -= ILoveVakuuPatch_ModifyAncientForAct;
        LoveVakuuPatches.ModifyAncientForAct += ILoveVakuuPatch_ModifyAncientForAct;
        LoveVakuuPatches.ModifyGenerateInitialOptions -= LoveVakuuPatches_ModifyGenerateInitialOptions;
        LoveVakuuPatches.ModifyGenerateInitialOptions += LoveVakuuPatches_ModifyGenerateInitialOptions;
    }

    protected override void AfterRunLoaded(RunState runState)
    {
        ActsToModify.Clear();
        ModifierOptions.Clear();
        LoveVakuuPatches.ModifyGenerateInitialOptions -= LoveVakuuPatches_ModifyGenerateInitialOptions;
        LoveVakuuPatches.ModifyGenerateInitialOptions += LoveVakuuPatches_ModifyGenerateInitialOptions;
    }

    private void LoveVakuuPatches_ModifyGenerateInitialOptions(object? sender, LoveVakuuPatches.ModifyInitialArgs e)
    {
        if (RunState.CurrentRoomCount > 1)
        {
            return;
        }
        ModifierOptions.Clear();
        foreach (ModifierModel modifier in RunState.Modifiers)
        {
            Func<Task>? option = modifier.GenerateNeowOption(e.Vakuu);
            if (option is null)
            {
                continue;
            }

            int index = ModifierOptions.Count;
            ModifierOptions.Add(new EventOption(e.Vakuu, () => OnChosen(option, index, e.Vakuu), modifier.NeowOptionTitle, modifier.NeowOptionDescription, modifier.Id.Entry, modifier.HoverTips));
        }

        if (ModifierOptions.Count <= 0)
        {
            return;
        }
        OriginalVakuuOptions.Add(e.Vakuu.Owner!.NetId, e.NewList);
        e.NewList = [ModifierOptions[0]];
    }

    private async Task OnChosen(Func<Task> option, int index, Vakuu vakuu)
    {
        await option();

        MethodInfo? method = AccessTools.DeclaredMethod(typeof(EventModel), "SetEventState", [typeof(LocString), typeof(IReadOnlyList<EventOption>)]);


        if (index + 1 >= ModifierOptions.Count)
        {
            method.Invoke(vakuu, [vakuu.InitialDescription, OriginalVakuuOptions[vakuu.Owner!.NetId]]);
        }
        else
        {
            IReadOnlyList<EventOption> next = [ModifierOptions[index + 1]];
            method.Invoke(vakuu, [vakuu.InitialDescription, next]);
        }
    }

    private void ILoveVakuuPatch_ModifyAncientForAct(object? sender, LoveVakuuPatches.AncientEventArgs e)
    {
        if (sender is not ActModel act)
        {
            return;
        }

        if (!ActsToModify.Remove(act, out RunState? _))
        {
            return;
        }

        if (e.NewAncient is Neow)
        {
            Vakuu ancientEventModel = ModelDb.AncientEvent<Vakuu>();
            e.NewAncient = ancientEventModel;
        }
    }
}