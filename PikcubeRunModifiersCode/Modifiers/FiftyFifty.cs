using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class FiftyFifty : PikcubeModifier
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (combatState.RoundNumber != 1)
        {
            return;
        }

        if (player.PlayerCombatState?.DrawPile is null)
        {
            return;
        }

        await Task.WhenAll(player.PlayerCombatState.DrawPile.Cards.Where((c, index) => index % 2 == 1).ToArray()
            .Select(async (card, index) =>
            {
                try
                {
                    for (int n = 0; n < index; ++n)
                    {
                        await Cmd.Wait(0.1f);
                    }

                    //Chairon's Ashes being an automatic win button is only fun the first time, so let's avoid triggering on exhaust hooks
                    CardPileAddResult cardPileAddResult = await CardPileCmd.Add(card, PileType.Exhaust);
                    CombatManager.Instance.History.CardExhausted(combatState, card);
                }
                catch (Exception e)
                {
                    MainFile.Logger.Error($"{e.GetType().Name}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}");
                }
            }));
    }
}