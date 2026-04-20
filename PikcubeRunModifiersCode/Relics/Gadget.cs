using BaseLib.Utils;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;
using System.Data;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class Gadget : PikcubeRunModifiersRelic
{
    private readonly List<SerializableCard> _cachedCardsPlayed = [];

    static Gadget()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(Gadget));
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        BetterHooks.AfterRunLoadedFromSave += BetterHooks_AfterRunLoadedFromSave;
        BetterHooks.AfterCreatingNewRun += BetterHooks_AfterCreatingNewRun;
    }

    private void BetterHooks_AfterCreatingNewRun(RunState runState, IReadOnlyList<Player> players, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, GameMode gameMode, int ascensionLevel, string seed)
    {
        GetGadget();
    }

    private void BetterHooks_AfterRunLoadedFromSave(RunState runState, SerializableRun serializableRun)
    {
        if (!IsMutable)
        {
            return;
        }
        GetGadget();
        CardsPlayedThisCombat.Clear();
        if (_cachedCardsPlayed.Count == 0)
        {
            return;
        }
        foreach (CardModel card in SerializableCard.LoadCardsFromPileIfExist(_cachedCardsPlayed, Owner.Deck))
        {
            CardsPlayedThisCombat.Add(card, card);
        }
    }

    private List<Task> PostLoadQueue { get; } = [];

    public static RelicModel Canonical => ModelDb.GetById<Gadget>(ModelDb.GetId<Gadget>()).CanonicalInstance;

    [SavedProperty]
    public SerializableCard? CurrentGadgetBlueprint
    {
        get;
        set;
    }

    [SavedProperty]
    public List<SerializableCard> SaveableCardsPlayedThisCombat
    {
        get => [.. CardsPlayedThisCombat.Values.Select(c => c.ToSerializable())];
        [UsedImplicitly]
        set => _cachedCardsPlayed.AddRange(value);
    }

    public CardModel? CurrentGadget
    {
        get;
        set;
    }

    public bool InitComplete { get; set; }

    public Task<CardModel?> ScrapACardTask { get; set; } = Task.FromResult<CardModel?>(null);

    private Dictionary<CardModel, CardModel> CardsPlayedThisCombat { get; } = [];

    [SavedProperty]
    public int TurnCoutner { get; set; }

    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new StringVar("WhenGadget", "At the start of each turn"),
        new StringVar("WhatGadget", "play [gold]Jackpot[/gold]")
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        CurrentGadgetCardHoverTip is null
        ? [ScrapTip]
        : [ScrapTip, CurrentGadgetCardHoverTip];

    private static HoverTip ScrapTip => new(ScrapName, ScrapDescription);

    private static LocString ScrapName => LocString.GetIfExists(_locTable, "PIKCUBERUNMODIFIERS-GADGET.scrapKeywordName")
                                          ?? throw new NoNullAllowedException();
    private static LocString ScrapDescription => LocString.GetIfExists(_locTable, "PIKCUBERUNMODIFIERS-GADGET.scrapKeyword")
                                                 ?? throw new NoNullAllowedException();
    public CardHoverTip? CurrentGadgetCardHoverTip { get; set; } = new(Jackpot.CreateWithoutOwner());


    public string WhenGadget
    {
        get => ((StringVar)DynamicVars["WhenGadget"]).StringValue;
        set => ((StringVar)DynamicVars["WhenGadget"]).StringValue = value;
    }
    public string WhatGadget
    {
        get => ((StringVar)DynamicVars["WhatGadget"]).StringValue;
        set => ((StringVar)DynamicVars["WhatGadget"]).StringValue = value;
    }

    public override Task AfterObtained()
    {
        InitDefault();
        return Task.CompletedTask;
    }


    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        CardsPlayedThisCombat.Clear();
        TurnCoutner = 0;
        return Task.CompletedTask;
    }

    public override Task BeforeCombatStart()
    {
        CardsPlayedThisCombat.Clear();
        TurnCoutner = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner != player)
        {
            return;
        }

        ++TurnCoutner;

        await PlayGadgetAsync(choiceContext, TurnCoutner);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.DupeOf == CurrentGadget || cardPlay.Card.DeckVersion is null)
        {
            return Task.CompletedTask;
        }

        CardsPlayedThisCombat.TryAdd(cardPlay.Card, cardPlay.Card.DeckVersion);
        return Task.CompletedTask;
    }

    public override bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        if (Owner.Relics.OfType<Gadget>().First() != this)
        {
            return false;
        }

        if (TurnCoutner == 0)
        {
            return false;
        }

        GetScrapOptions();
        return false;
    }

    public override async Task BeforeRewardsOffered(Player player, IReadOnlyList<Reward> rewards)
    {
        if (Owner.Relics.OfType<Gadget>().First() != this)
        {
            return;
        }

        if (TurnCoutner == 0)
        {
            return;
        }

        CardModel? card = await ScrapACardTask;
        ScrapACardTask = Task.FromResult<CardModel?>(null);
        if (card is not null)
        {
            await UpdateGadget(card);
        }

        await DoNext();
    }

    private async Task DoNext()
    {
        using IEnumerator<Gadget> itterator = Owner.Relics.OfType<Gadget>().GetEnumerator();
        while (itterator.MoveNext())
        {
            if (itterator.Current != this)
            {
                continue;
            }

            break;
        }

        if (itterator.MoveNext())
        {
            await itterator.Current.DoScrap();
        }
    }

    private async Task DoScrap()
    {
        if (TurnCoutner == 0)
        {
            return;
        }
        GetScrapOptions();
        CardModel? card = await ScrapACardTask;
        ScrapACardTask = Task.FromResult<CardModel?>(null);
        if (card is not null)
        {
            await UpdateGadget(card);
        }
        await DoNext();
    }

    private void InitDefault()
    {
        CurrentGadgetBlueprint = Jackpot.CreateInstance(Owner).ToSerializable();
        InitComplete = true;
        GetGadget();
    }

    private void GetGadget()
    {
        CardModel? card = GetGadgetInfo(CurrentGadgetBlueprint, Owner, out string when, out string what, out CardHoverTip? tip);
        WhenGadget = when;
        WhatGadget = what;
        CurrentGadgetCardHoverTip = tip;
        CurrentGadget = card;
    }

    private async Task PlayGadgetAsync(PlayerChoiceContext choiceContext, int turnCoutner)
    {
        while (CurrentGadget is null)
        {
            GetGadget();
        }

        await DoGadgetEffectAsync(choiceContext, CurrentGadget, turnCoutner, this);

    }

    private void GetScrapOptions()
    {
        List<CardModel> scrapChoices =
        [
            ..Owner.Deck.Cards
                .Where(card => card.IsRemovable)
                .Where(card => card.Rarity is not CardRarity.Ancient)
                .Where(card => card.Type is CardType.Attack or CardType.Skill or CardType.Power)
        ];

        Owner.RunState.Rng.Niche.Shuffle(scrapChoices);

        CardModel[] cards =
        [
            ..scrapChoices
                .OrderByDescending(c => CardsPlayedThisCombat.ContainsValue(c))
                .Take(3)
        ];


        if (cards.Length == 0)
        {
            return;
        }

        PlayerChoiceContext context = new BlockingPlayerChoiceContext();
        ScrapACardTask = CardSelectCmd.FromChooseACardScreen(context, cards, Owner);
    }


    private static async Task DoGadgetEffectAsync(PlayerChoiceContext choiceContext, CardModel currentGadget, int turnCoutner, Gadget? gadget)
    {
        if (currentGadget.Rarity == CardRarity.Ancient || currentGadget.Keywords.Contains(CardKeyword.Unplayable))
        {
            return;
        }

        if (currentGadget.Type == CardType.Power || currentGadget.Keywords.Contains(CardKeyword.Exhaust))
        {
            if (turnCoutner > 1)
            {
                return;
            }

            CardModel card = currentGadget.CreateDupe();
            card.AddKeyword(CardKeyword.Retain);
            if (!card.EnergyCost.CostsX && !card.HasStarCostX)
            {
                card.SetToFreeThisCombat();
            }

            gadget?.Flash();
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true, CardPilePosition.Top);

            return;
        }

        if (currentGadget.HasStarCostX)
        {
            currentGadget.LastStarsSpent = turnCoutner;
        }

        if (currentGadget.EnergyCost.CostsX)
        {
            currentGadget.EnergyCost.CapturedXValue = turnCoutner;
        }

        gadget?.Flash();
        await CardCmd.AutoPlay(choiceContext, currentGadget.CreateDupe(), null);
    }

    private static CardModel? GetGadgetInfo(SerializableCard? currentGadgetBlueprint, Player owner,
        out string whenGadget, out string whatGadget, out CardHoverTip? currentGadgetCardHoverTip)
    {
        if (currentGadgetBlueprint?.Id is null)
        {
            whenGadget = "Does nothing";
            whatGadget = "at the end of combat, scrap a card to make a gadget";
            currentGadgetCardHoverTip = null;
            return null;
        }

        CardModel card = currentGadgetBlueprint.CreateNewInstance(owner);
        GetGadgetInfo(card, out whenGadget, out whatGadget, out currentGadgetCardHoverTip);

        return card;
    }

    private static void GetGadgetInfo(CardModel? card, out string whenGadget, out string whatGadget,
        out CardHoverTip? currentGadgetCardHoverTip)
    {
        switch (card?.Rarity)
        {
            case CardRarity.Ancient:
                whenGadget = "Does nothing";
                whatGadget = "Ancient cards are unscrappable.";
                currentGadgetCardHoverTip = null;
                return;
            case CardRarity.Curse:
                whenGadget = "Does nothing";
                whatGadget = "Curse cards are unscrappable.";
                currentGadgetCardHoverTip = null;
                return;
            case CardRarity.Status:
                whenGadget = "Does nothing";
                whatGadget = "Status cards are unscrappable.";
                currentGadgetCardHoverTip = null;
                return;
            case CardRarity.Quest:
                whenGadget = "Does nothing";
                whatGadget = "Quest cards are unscrappable.";
                currentGadgetCardHoverTip = null;
                return;
            case CardRarity.None:
            case CardRarity.Basic:
            case CardRarity.Common:
            case CardRarity.Uncommon:
            case CardRarity.Rare:
            case CardRarity.Event:
            case CardRarity.Token:
                currentGadgetCardHoverTip = new CardHoverTip(card);

                if (card.Keywords.Contains(CardKeyword.Unplayable))
                {
                    whenGadget = "Does nothing";
                    whatGadget = "this card can't be played.";
                    return;
                }

                if (card.Type == CardType.Power || card.Keywords.Contains(CardKeyword.Exhaust))
                {
                    whenGadget = "At the start of combat";
                    whatGadget = $"add a copy of [gold]{card.GetTrueTitle()}[/gold] to your hand. " +
                                 $"It [gold]Retains[/gold] and is free to play.";
                    return;
                }

                if (card.EnergyCost.CostsX || card.HasStarCostX)
                {
                    whenGadget = "At the start of each turn";
                    whatGadget = $"play [gold]{card.GetTrueTitle()}[/gold] with X = Turn Number.";
                    return;
                }

                whenGadget = "At the start of each turn";
                whatGadget = $"play [gold]{card.GetTrueTitle()}[/gold]";
                return;
            case null:
                whenGadget = "Does nothing";
                whatGadget = "at the end of combat, scrap a card to make a gadget.";
                currentGadgetCardHoverTip = null;
                return;
            default:
                whenGadget = "Does nothing";
                whatGadget = $"{nameof(card.Type)} cards are unscrappable due to not being a know type.";
                currentGadgetCardHoverTip = null;
                return;
        }



    }

    public static string GetGadgetTip(CardModel instance)
    {
        GetGadgetInfo(instance, out string when, out string what, out _);
        return $"{when}, {what}";
    }

    private async Task UpdateGadget(CardModel card)
    {
        await CardPileCmd.RemoveFromDeck(card, false);
        CurrentGadgetBlueprint = card.ToSerializable();
        GetGadget();
    }
}