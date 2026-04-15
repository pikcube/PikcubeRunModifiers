using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Enchantments;

public class Sticky : CustomEnchantmentModel
{
    protected override string? CustomIconPath => $"res://{MainFile.ModId}/images/enchantments/{nameof(Sticky).ToLowerInvariant()}.png";

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Innate)
    ];
    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Innate);
    }
}