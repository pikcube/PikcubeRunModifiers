using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Relics;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class GadgetsModifier : PikcubeModifier
{

    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is Gadget))
            {
                continue;
            }

            if (RunManager.Instance.NetService.NetId != p.NetId)
            {
                continue;
            }
            RelicCmd.Obtain<Gadget>(p);
            SaveManager.Instance.MarkRelicAsSeen(Gadget.Canonical);
        }
    }
}