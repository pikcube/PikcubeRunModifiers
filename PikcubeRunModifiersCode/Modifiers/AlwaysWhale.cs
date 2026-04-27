using System.Reflection;
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
public class AlwaysWhale : PikcubeModifier
{
    private List<EventOption> ModifierOptions { get; } = [];
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
        AlwaysWhalePatches.ModifyGenerateInitialOptions -= AlwaysWhalePatches_ModifyGenerateInitialOptions;
        if (RunState.CurrentRoomCount > 1)
        {
            return;
        }
        ModifierOptions.Clear();
        foreach (ModifierModel modifier in RunState.Modifiers)
        {
            Func<Task>? option = modifier.GenerateNeowOption(e.Neow);
            if (option is null)
            {
                continue;
            }

            int index = ModifierOptions.Count;
            ModifierOptions.Add(new EventOption(e.Neow, () => OnChosen(option, index, e.Neow), modifier.NeowOptionTitle, modifier.NeowOptionDescription, modifier.Id.Entry, modifier.HoverTips));
        }
        if (ModifierOptions.Count == 0)
        {
            e.NewList = AlwaysWhalePatches.NeowReverseOptionsPatch.GenerateInitialOptionsWithoutModifiers(e.Neow);
            return;
        }

        e.NewList = [ModifierOptions[0]];

    }

    private async Task OnChosen(Func<Task> option, int index, Neow neow)
    {
        await option();

        MethodInfo? method = AccessTools.DeclaredMethod(typeof(EventModel), "SetEventState", [typeof(LocString), typeof(IReadOnlyList<EventOption>)]);


        if (index + 1 >= ModifierOptions.Count)
        {
            method.Invoke(neow, [neow.InitialDescription, AlwaysWhalePatches.NeowReverseOptionsPatch.GenerateInitialOptionsWithoutModifiers(neow)]);
        }
        else
        {
            IReadOnlyList<EventOption> next = [ModifierOptions[index + 1]];
            method.Invoke(neow, [neow.InitialDescription, next]);
        }
    }
}