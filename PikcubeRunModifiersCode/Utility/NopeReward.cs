using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

public class NopeReward(Player player, List<Reward> original, List<Reward> current)
    : Reward(player)
{
    [CustomEnum] 
    public static RewardType NopeRewardType = 0;

    public List<Reward> Original { get; } = original;

    protected override string IconPath => $"reward/pikcube.nope.png".ImagePath();

    public override void Populate()
    {
    }

    protected override async Task<bool> OnSelect()
    {
        await FortuneFavorsTheBold.ModifyAsync(Player, Original, current);
        return true;
    }

    public override void MarkContentAsSeen()
    {
    }

    protected override RewardType RewardType => NopeRewardType;
    public override int RewardsSetIndex => 10;
    public override LocString Description => new("modifiers", "PIKCUBERUNMODIFIERS-FORTUNE_FAVORS_THE_BOLD.rewardNopeDescription");
    public override bool IsPopulated => true;
}