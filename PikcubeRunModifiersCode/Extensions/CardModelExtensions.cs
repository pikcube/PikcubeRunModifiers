using MegaCrit.Sts2.Core.Models;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

public static class CardModelExtensions
{
    public static string GetTrueTitle<T>(this T card) where T : CardModel =>
        $"{card.TitleLocString.GetFormattedText()}{(card.IsUpgraded ? "+" : "")}";
}