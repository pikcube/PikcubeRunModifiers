using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

public static class CardModelExtensions
{
    public static string GetTrueTitle<T>(this T card) where T : CardModel =>
        $"{card.TitleLocString.GetFormattedText()}{(card.IsUpgraded ? "+" : "")}";
}