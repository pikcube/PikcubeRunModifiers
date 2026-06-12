using System.Data;
using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class AlwaysWhale : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;
    public override int SortOrder => -999;

    private Dictionary<ulong, List<EventOption>> ModifierOptions { get; } = [];
    protected override void AfterRunCreated(RunState runState)
    {
        ModifierOptions.Clear();
        AlwaysWhalePatches.ModifyGenerateInitialOptions -= AlwaysWhalePatches_ModifyGenerateInitialOptions;
        AlwaysWhalePatches.ModifyGenerateInitialOptions += AlwaysWhalePatches_ModifyGenerateInitialOptions;
    }

    protected override void AfterRunLoaded(RunState runState)
    {
        ModifierOptions.Clear();
        AlwaysWhalePatches.ModifyGenerateInitialOptions -= AlwaysWhalePatches_ModifyGenerateInitialOptions;
        AlwaysWhalePatches.ModifyGenerateInitialOptions += AlwaysWhalePatches_ModifyGenerateInitialOptions;
    }

    private void AlwaysWhalePatches_ModifyGenerateInitialOptions(object? sender, AlwaysWhalePatches.ModifyInitialArgs e)
    {
        if (RunState.CurrentRoomCount > 1 || e.Neow.Owner is null)
        {
            return;
        }
        ModifierOptions[e.Neow.Owner.NetId] = [];
        foreach (ModifierModel modifier in RunState.Modifiers)
        {
            Func<Task>? option = modifier.GenerateNeowOption(e.Neow);
            if (option is null)
            {
                continue;
            }

            int index = ModifierOptions[e.Neow.Owner.NetId].Count;
            ModifierOptions[e.Neow.Owner.NetId].Add(new EventOption(e.Neow, () => OnChosen(option, index, e.Neow), modifier.NeowOptionTitle, modifier.NeowOptionDescription, modifier.Id.Entry, modifier.HoverTips));
        }
        if (ModifierOptions[e.Neow.Owner.NetId].Count == 0)
        {
            e.NewList = AlwaysWhalePatches.NeowReverseOptionsPatch.GenerateInitialOptionsWithoutModifiers(e.Neow);
            return;
        }

        e.NewList = [ModifierOptions[e.Neow.Owner.NetId][0]];

    }

    private async Task OnChosen(Func<Task> option, int index, Neow neow)
    {
        await option();

        MethodInfo? method = AccessTools.DeclaredMethod(typeof(EventModel), "SetEventState", [typeof(LocString), typeof(IReadOnlyList<EventOption>)]);

        if (neow.Owner is null)
        {
            throw new NoNullAllowedException();
        }

        if (index + 1 >= ModifierOptions[neow.Owner.NetId].Count)
        {
            method.Invoke(neow, [neow.InitialDescription, AlwaysWhalePatches.NeowReverseOptionsPatch.GenerateInitialOptionsWithoutModifiers(neow)]);
        }
        else
        {
            IReadOnlyList<EventOption> next = [ModifierOptions[neow.Owner.NetId][index + 1]];
            method.Invoke(neow, [neow.InitialDescription, next]);
        }
    }

    public override IEnumerable<ModifierModel> MutuallyExclusiveGroup => [ModelDb.Modifier<TheILoveVakuuModifier>()];
}