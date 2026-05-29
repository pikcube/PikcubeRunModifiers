using System.Data;
using BaseLib.Abstracts;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class Heirloom : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

    public override LocString NeowOptionTitle => Title;
    public override LocString NeowOptionDescription => Description;
    
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return async () =>
        {
            Player p = eventModel.Owner ?? throw new NoNullAllowedException();
            RelicModel pullFromFront = p.RelicGrabBag.PullFromFront(RelicRarity.Rare, RunState) ?? ModelDb.Relic<Circlet>();
            await RelicCmd.Obtain(pullFromFront.ToMutable(), p);
        };
    }
}