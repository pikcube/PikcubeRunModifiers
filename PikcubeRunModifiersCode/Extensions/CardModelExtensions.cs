using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

public static class CardModelExtensions
{
    extension<T>(T card) where T : CardModel
    {
        public string GetTrueTitle()
        {
            return card.IsUpgraded
                ? $"{card.TitleLocString.GetFormattedText()}+"
                : card.TitleLocString.GetFormattedText();
        }
    }
}