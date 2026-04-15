using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class FiftyFifty : PikcubeModifier
{
    public override bool ShouldReceiveCombatHooks => true;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
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
            .Select(async (c, index) =>
            {
                try
                {
                    for (int n = 0; n < index; ++n)
                    {
                        await Cmd.Wait(0.1f);
                    }
                    await CardCmd.Exhaust(choiceContext, c);
                }
                catch (Exception e)
                {
                    //It's not like there's anything to do at this point but hope and pray this was thrown because Chairon's Ashes ended combat
                    MainFile.Logger.Error($"{e.GetType().Name}{Environment.NewLine}{e.Message}{Environment.NewLine}{e.StackTrace}");
                }
            }));
    }
}