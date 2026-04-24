using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using System.Data;
using MegaCrit.Sts2.Core.Entities.Players;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

public class LawCardReward(CardReward baseCardReward, CardModel target) : CardReward(baseCardReward.Cards,
    baseCardReward.GetOptions().Source, baseCardReward.Player, baseCardReward.GetOptions())
{
    private CardModel Target { get; } = target;

    protected override async Task<bool> OnSelect()
    {
        await GetLawedFunc([.. Cards], Target, baseCardReward.Player);

        return true;
    }

    public static async Task GetLawedFunc(IList<CardModel> cards, CardModel target, Player player)
    {
        List<CardPileAddResult> results = [];
        foreach (CardModel card in cards.Where(c => c.CanonicalInstance == target.CanonicalInstance))
        {
            results.Add(await CardPileCmd.Add(card, PileType.Deck));
        }

        foreach ((int i, CardModel card) in cards.Index())
        {
            float delayTimeBasedOnIndex = i * 0.5f + 0.2f;
            if (card.CanonicalInstance == target.CanonicalInstance)
            {
                CardPileAddResult result = results.Single(c => c.cardAdded == card);
                CardCmd.PreviewCardPileAdd(result, delayTimeBasedOnIndex);
            }
            else
            {
                player.RunState.CurrentMapPointHistoryEntry?.GetEntry(LocalContext.NetId ?? 0).CardChoices.Add(new CardChoiceHistoryEntry(card, false));
                RunManager.Instance.RewardSynchronizer.SyncLocalSkippedCard(card);
                ShowAndDestoryCard(card, delayTimeBasedOnIndex);
            }
        }
    }

    private static void ShowAndDestoryCard(CardModel card, float delayTimeBasedOnIndex)
    {
        Control cardPreviewContainer = NRun.Instance?.GlobalUi.CardPreviewContainer ?? throw new NoNullAllowedException();
        NCard nCard = NCard.Create(card) ?? throw new NoNullAllowedException();
        cardPreviewContainer.AddChildSafely(nCard);
        nCard.UpdateVisuals(PileType.Exhaust, CardPreviewMode.Normal);
        Tween tween = nCard.CreateTween();
        tween.TweenProperty(nCard, (NodePath)"scale", Vector2.One, 0.25)
            .From(Vector2.Zero)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        tween.TweenInterval(delayTimeBasedOnIndex);
        tween.TweenCallback(Callable.From((Action)(() => { NRun.Instance?.GlobalUi.AddChildSafely(NExhaustVfx.Create(nCard)!); })));
        tween.TweenProperty(nCard, (NodePath)"modulate", StsColors.exhaustGray, 
            SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.20000000298023224 : 0.30000001192092896);
        tween.TweenCallback(Callable.From(nCard.QueueFree));
    }
}