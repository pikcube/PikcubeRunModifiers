using MegaCrit.Sts2.Core.Models;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

public static class CardModelExtensions
{
    extension<T>(T card) where T : CardModel
    {
        public string GetTrueTitle() => $"{card.TitleLocString.GetFormattedText()}{(card.IsUpgraded ? "+" : "")}";
    }
}