using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;
using System.Data;
using System.Reflection;
using System.Text.Json;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public abstract class TheAbstractLaw(CustomRunType type, string name) : PikcubeRunModifierModel(type, name)
{
    public abstract Func<CardModel, bool> Filter { get;}

    [SavedProperty]
    public string DicKeys {
        get => JsonSerializer.Serialize(LawCardBlueprint);
        set => LawCardBlueprint = JsonSerializer.Deserialize<Dictionary<ulong, SerializableCard>>(value) ?? throw new NoNullAllowedException();
    }

    public Dictionary<ulong, bool> Rigged { get; } = [];

    protected override void AfterRunCreated(RunState runState)
    {
        LawCardBlueprint.Clear();
    }

    public Dictionary<ulong, SerializableCard> LawCardBlueprint { get; set; } = [];

    public override Func<Task>? GenerateNeowOption(EventModel eventModel)
    {
        return async () =>
        {
            Player p = eventModel.Owner ?? throw new NoNullAllowedException();

            CardModel? strike = p.Deck.Cards.FirstOrDefault(c => c.Tags.Contains(CardTag.Strike) && c.Rarity == CardRarity.Basic);
            if (strike is null)
            {
                return;
            }

            CardModel? defend = p.Deck.Cards.FirstOrDefault(c => c.Tags.Contains(CardTag.Defend) && c.Rarity == CardRarity.Basic);
            if (defend is null)
            {
                return;
            }

            CardModel? added = (await CardSelectCmd.FromSimpleGrid(new BlockingPlayerChoiceContext(),
                [.. p.Character.CardPool.AllCards.Where(Filter)], p,
                new CardSelectorPrefs(LocString.GetIfExists("modifiers", "THE_LAW.add")!, 1))).SingleOrDefault();


            if (added is null)
            {
                //wut?
                return;
            }

            await CardCmd.Transform(strike, p.RunState.CreateCard(added.CanonicalInstance, p), CardPreviewStyle.GridLayout);
            await CardCmd.Transform(defend, p.RunState.CreateCard(added.CanonicalInstance, p), CardPreviewStyle.GridLayout);

            LawCardBlueprint.Add(p.NetId, p.RunState.CreateCard(added.CanonicalInstance, p).ToSerializable());
        };
    }

    public override bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)
    {
        Rigged.TryGetValue(player.NetId, out bool isRigged);
        if (!isRigged)
        {
            return false;
        }

        if (!LawCardBlueprint.TryGetValue(player.NetId, out SerializableCard? blueprint))
        {
            return false;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

        Rigged[player.NetId] = false;

        if (cardRewardOptions.Any(c => c.Card.CanonicalInstance == lawCard.CanonicalInstance))
        {
            return false;
        }

        CardCreationResult optionToRig = cardRewardOptions[1];
        optionToRig.ModifyCard(lawCard.CreateNewInstance(player));
        return true;
    }

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        bool modified = false;

        if (!LawCardBlueprint.TryGetValue(player.NetId, out SerializableCard? blueprint))
        {
            return modified;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

        foreach (CardReward cardReward in rewards.OfType<CardReward>().ToArray())
        {
            if (!cardReward.IsPopulated)
            {
                //Warning, never do this, I just happen to know this function always runs synchronously
                cardReward.Populate().GetAwaiter().GetResult();
            }

            CardModel? lawCardRewardItem = cardReward.Cards.FirstOrDefault(c => c.CanonicalInstance == lawCard.CanonicalInstance);

            if (lawCardRewardItem is null)
            {
                continue;
            }

            rewards.TryReplaceValue(cardReward, new LawCardReward(cardReward, lawCardRewardItem));
            modified = true;
        }


        return modified;
    }

    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward,
        List<CardRewardAlternative> alternatives)
    {
        if (!LawCardBlueprint.TryGetValue(player.NetId, out SerializableCard? blueprint))
        {
            return false;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

        CardRewardAlternative? reroll = alternatives.FirstOrDefault(alt => alt.OptionId == "REROLL");
        if (reroll is null)
        {
            return false;
        }

        CardRewardAlternative newReroll = new("REROLL", async () =>
        {
            await cardReward.Reroll();

            CardModel? lawCardRewardItem =
                cardReward.Cards.FirstOrDefault(c => c.CanonicalInstance == lawCard.CanonicalInstance);

            if (lawCardRewardItem is null)
            {
                return;
            }

            await LawCardReward.GetLawedFunc([.. cardReward.Cards], lawCard, player);
            FieldInfo? screenFieldInfo = AccessTools.DeclaredField(typeof(CardReward), "_currentlyShownScreen");
            NCardRewardSelectionScreen? screen = (NCardRewardSelectionScreen?)screenFieldInfo.GetValue(cardReward);
            if (screen is null)
            {
                return;
            }

            FieldInfo? taskCompletionInfo = AccessTools.DeclaredField(typeof(NCardRewardSelectionScreen), "_completionSource");
            TaskCompletionSource<Tuple<IEnumerable<NCardHolder>, bool>>? taskSource = (TaskCompletionSource<Tuple<IEnumerable<NCardHolder>, bool>>?)taskCompletionInfo.GetValue(screen);
            if (taskSource is null)
            {
                return;
            }

            taskSource.SetResult(new Tuple<IEnumerable<NCardHolder>, bool>([], true));
        }, PostAlternateCardRewardAction.DoNothing);
        alternatives.TryReplaceValue(reroll, newReroll);

        return true;
    }

    public void RigNext(Player issuingPlayer)
    {
        Rigged[issuingPlayer.NetId] = true;
    }
}