using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

[UsedImplicitly]
public class TheLaw : TheAbstractLaw
{
    static TheLaw()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(TheLaw));
    }

    public override Func<CardModel, bool> Filter => (c => c.Rarity == CardRarity.Common);
}