using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;
using System.Data;
using System.Reflection;
using System.Text.Json;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using Pikcube.Common.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class TheLaw() : PikcubeRunModifierModel(CustomRunType.Good, "The Law")
{
    static TheLaw()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(TheLaw));
    }

    public static Func<CardModel, bool> Filter => (c => c.Rarity == CardRarity.Common);

    [SavedProperty]
    public string DicKeys
    {
        get => JsonSerializer.Serialize(LawCardBlueprint);
        set => LawCardBlueprint = JsonSerializer.Deserialize<Dictionary<ulong, SerializableCard>>(value) ?? throw new NoNullAllowedException();
    }

    public override void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)
    {
        Rigged.TryGetValue(player.NetId, out bool isRigged);
        Rigged[player.NetId] = false;
        if (!isRigged)
        {
            return;
        }

        if (!LawCardBlueprint.TryGetValue(player.NetId, out SerializableCard? blueprint))
        {
            return;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

        if (cards.Any(c => c.Card.CanonicalInstance == lawCard.CanonicalInstance))
        {
            return;
        }

        CardCreationResult optiontoRig = cards.First(c => c.Card.Type == lawCard.Type);
        optiontoRig.ModifyCard(lawCard.CreateNewInstance(player));
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not MerchantRoom merchantRoom || !LawCardBlueprint.TryGetValue(merchantRoom.Inventory.Player.NetId, out SerializableCard? blueprint))
        { 
            return Task.CompletedTask;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

        if (merchantRoom.Inventory.CharacterCardEntries.Any(cardEntry =>
                (cardEntry.CreationResult?.Card.CanonicalInstance == lawCard.CanonicalInstance) & cardEntry.EnoughGold))
        {
            TaskHelper.RunSafely(PurchaseLawCard(merchantRoom.Inventory, lawCard.CanonicalInstance));
        }
        return Task.CompletedTask;
    }

    //Code shamelessly stolen from lord's parasol
    private static async Task PurchaseLawCard(MerchantInventory inventory, CardModel lawCardCanonicalInstance)
    {
        bool uiBlocked = false;
        NRun? nRun = NRun.Instance;
        NMapScreen? nMapScreen = NMapScreen.Instance;
        NMerchantRoom? nMerchantRoom = NMerchantRoom.Instance;
        if (nRun is null || nMapScreen is null || nMerchantRoom is null)
        {
            return;
        }

        try
        {
            if (TestMode.IsOff)
            {
                nRun.GlobalUi.TopBar.Map.Disable();
                nRun.GlobalUi.TopBar.Deck.Disable();
                nMapScreen.SetTravelEnabled(false);
                double num = await nRun.AwaitProcessFrame();
                uiBlocked = true;
                nMerchantRoom.Inventory.BlockInput();
                await Cmd.Wait(0.75f);
                nMerchantRoom.Inventory.Open();
                await Cmd.Wait(1f);
            }

            foreach (MerchantCardEntry characterCardEntry in inventory.CharacterCardEntries.Where(c =>
                         c.CreationResult?.Card.CanonicalInstance == lawCardCanonicalInstance))
            {
                if (!characterCardEntry.IsStocked || !characterCardEntry.EnoughGold)
                {
                }
                else
                {
                    await characterCardEntry.OnTryPurchaseWrapper(inventory);
                    await Cmd.Wait(0.25f);
                }
            }

            foreach (MerchantCardEntry colorlessCardEntry in inventory.ColorlessCardEntries.Where(c =>
                         c.CreationResult?.Card.CanonicalInstance == lawCardCanonicalInstance))
            {
                if (!colorlessCardEntry.IsStocked || !colorlessCardEntry.EnoughGold)
                {
                }
                else
                {
                    await colorlessCardEntry.OnTryPurchaseWrapper(inventory);
                    await Cmd.Wait(0.25f);
                }
            }
        }
        finally
        {
            if (uiBlocked)
            {
                nMerchantRoom.Inventory.UnblockInput();
                nRun.GlobalUi.TopBar.Map.Enable();
                nRun.GlobalUi.TopBar.Deck.Enable();
                nMapScreen.SetTravelEnabled(true);
            }
        }

        nMapScreen.SetTravelEnabled(true);
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
        Rigged[player.NetId] = false;

        if (!isRigged)
        {
            return false;
        }

        if (!LawCardBlueprint.TryGetValue(player.NetId, out SerializableCard? blueprint))
        {
            return false;
        }

        CardModel lawCard = CardModel.FromSerializable(blueprint);

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