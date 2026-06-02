using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;
using System.Text.Json;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

public class NopeReward(Player player, List<Reward> original, List<Reward> current)
    : CustomReward(player)
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
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    private static CustomReward Deserialize(SerializableReward save, Player player)
    {
        string? rewardString = save.SpecialCard?.Props?.strings?.First().value;
        string? rewardString2 = save.SpecialCard?.Props?.strings?.Last().value;
        if (rewardString is null || rewardString2 is null)
        {
            return new NopeReward(player, [], []);
        }

        List<SerializableReward> rewards = JsonSerializer.Deserialize<List<SerializableReward>>(rewardString) ?? [];
        List<SerializableReward> rewards2 = JsonSerializer.Deserialize<List<SerializableReward>>(rewardString2) ?? [];
        return new NopeReward(player, [.. rewards.Select(sr => FromSerializable(sr, player))],
            [..rewards2.Select(sr => FromSerializable(sr, player))]);
    }

    public override SerializableReward ToSerializable()
    {
        
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        // Apparently it is null‽
        if (Original is null || current is null) 
        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        {
            return new SerializableReward
            {
                RewardType = NopeRewardType,
                SpecialCard = new SerializableCard
                {
                    Props = new SavedProperties
                    {
                        strings = [new SavedProperties.SavedProperty<string>("pikcube.r", "{}"), new SavedProperties.SavedProperty<string>("pikcube.r2", "{}")]
                    }
                }
            };
        }

        string rewards = JsonSerializer.Serialize(Original.Select(r => r.ToSerializable()));
        string rewards2 = JsonSerializer.Serialize(current.Select(r => r.ToSerializable()));

        return new SerializableReward
        {
            RewardType = NopeRewardType,
            SpecialCard = new SerializableCard
            {
                Props = new SavedProperties
                {
                    strings = [new SavedProperties.SavedProperty<string>("pikcube.r", rewards), new SavedProperties.SavedProperty<string>("pikcube.r2", rewards2)]
                }
            }
        };
    }

    public override LocString Description => new("modifiers", "PIKCUBERUNMODIFIERS-FORTUNE_FAVORS_THE_BOLD.rewardNopeDescription");
    public override bool IsPopulated => true;
}