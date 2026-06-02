using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Patches;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

public class MinigameReward : Reward
{
    [CustomEnum] 
    public static RewardType CrystalBallReward = 0;

    protected override string IconPath => $"reward/pikcube.{(Price < 0 ? "debtball" : "moneyball")}.png".ImagePath();
    public List<Reward> Original { get; set; }

    public int Price { get; }

    public MinigameReward(Player player, List<Reward> original, int price) : base(player)
    {
        Original = original;
        Price = price;
        if (Price > 0)
        {
            return;
        }

        ModifyCrystalSphereRewardsPatch.ModifyCrystalSphereRewards += ModifyCrystalSphereRewardsPatch_ModifyCrystalSphereRewards;
        RunManager.Instance.RoomEntered += Instance_RoomEntered;
    }

    private void Instance_RoomEntered()
    {
        ModifyCrystalSphereRewardsPatch.ModifyCrystalSphereRewards -= ModifyCrystalSphereRewardsPatch_ModifyCrystalSphereRewards;
        RunManager.Instance.RoomEntered -= Instance_RoomEntered;
    }

    private void ModifyCrystalSphereRewardsPatch_ModifyCrystalSphereRewards(ref List<Reward> rewards, Player player)
    {
        if (player != Player)
        {
            return;
        }
        ModifyCrystalSphereRewardsPatch.ModifyCrystalSphereRewards -= ModifyCrystalSphereRewardsPatch_ModifyCrystalSphereRewards;
        rewards = [.. Original, .. rewards];
    }

    protected override async Task<bool> OnSelect()
    {
        if (Price > 0)
        {
            Player.Gold -= Price;
        }
        else
        {
            await CardPileCmd.AddCurseToDeck<Debt>(Player);
        }
        await new CrystalSphereMinigame(Player, Player.PlayerRng.Rewards, Price < 0 ? 6 : 3).PlayMinigame();
        return true;
    }
    public override void Populate()
    {
    }

    public override void MarkContentAsSeen()
    {
    }

    protected override RewardType RewardType => CrystalBallReward;
    public override int RewardsSetIndex => 10;

    public override LocString Description => Price < 0 ? GetDebtString() : GetGoldString();

    private LocString GetGoldString()
    {
        LocString goldString = new("modifiers", "PIKCUBERUNMODIFIERS-FORTUNE_FAVORS_THE_BOLD.rewardGoldDescription");
        goldString.Add(new DynamicVar("Gold", Price));
        return goldString;
    }

    private static LocString GetDebtString()
    {
        return new LocString("modifiers", "PIKCUBERUNMODIFIERS-FORTUNE_FAVORS_THE_BOLD.rewardDebtDescription");
    }

    public override bool IsPopulated => true;

    public override IEnumerable<IHoverTip> HoverTips => Price < 0 ? [HoverTipFactory.FromCard<Debt>()] : [];
}