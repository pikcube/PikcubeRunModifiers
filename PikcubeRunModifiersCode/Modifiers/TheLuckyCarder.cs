using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Rewards;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class TheLuckyCarder : PikcubeModifier
{
    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        rewards.Add(new CardBundleReward(new CardCreationOptions([player.Character.CardPool], CardCreationSource.Encounter, CardRarityOddsType.RegularEncounter), player, 2, 3));
        return true;
    }
}