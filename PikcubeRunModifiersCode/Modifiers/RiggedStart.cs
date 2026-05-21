using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Rewards;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class RiggedStart() : PikcubeRunModifierModel(CustomRunType.None, "Rigged Start")
{
    public override Func<Task> GenerateNeowOption(EventModel eventModel)
    {
        return async () =>
        {
            Player? p = eventModel.Owner;
            if (p is null)
            {
                return;
            }

            List<RelicModel> relics = [];

            if (eventModel is not Neow neow)
            {
                neow = ModelDb.Event<Neow>();
            }

            relics.AddRange(neow.AllPossibleOptions.Select(o => o.Relic).Where(r => r is not null).OfType<RelicModel>());

            LinkedRewardSet linkedRewards = new([.. relics.Select(r => new RelicReward(r, p))], p);

            await new RewardsSet(p).WithCustomRewards([linkedRewards]).Offer();
        };
    }
}