using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class OneOfEverything : PikcubeModifier
{
    public override bool ClearsPlayerDeck => true;

    public override Func<Task>? GenerateNeowOption(EventModel eventModel)
    {
        return async () =>
        {
            Player? player = eventModel.Owner;
            if (player is null)
            {
                return;
            }

            foreach (CardModel card in ModelDb.AllCards.Where(card => card.Type is CardType.Attack or CardType.Skill or CardType.Power or CardType.Curse && !card.Keywords.Contains(CardKeyword.Innate)))
            {
                CardPileAddResult cardPileAddResult = await CardPileCmd.Add(player.RunState.CreateCard(card, player), PileType.Deck);

                CardCmd.PreviewCardPileAdd(cardPileAddResult, style: CardPreviewStyle.MessyLayout);
                await Cmd.CustomScaledWait(0.01f, 0.02f);

            }
        };
    }
}