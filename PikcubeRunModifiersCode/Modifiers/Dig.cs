using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class Dig() : PikcubeRunModifierModel(CustomRunType.Good, "Dig!")
{
    static Dig()
    {
        new RelicSpawnManager().RegisterRule<Shovel>(Predicates.UnlessModifierPresent<Dig>);
    }
    
    protected override void AfterRunCreated(RunState runState)
    {
        foreach (Player p in runState.Players)
        {
            if (p.Relics.Any(r => r is Shovel))
            {
                continue;
            }
            RelicCmd.Obtain<Shovel>(p);
        }
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        HealRestSiteOption? heal = options.OfType<HealRestSiteOption>().FirstOrDefault();
        if (heal is null)
        {
            return false;
        }

        heal.IsEnabled = false;

        MendRestSiteOption? mend = options.OfType<MendRestSiteOption>().FirstOrDefault();
        if (mend is null)
        {
            return true;
        }

        mend.IsEnabled = false;

        return true;
    }
}