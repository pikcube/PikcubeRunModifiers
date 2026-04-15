using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class AboveTheLaw : TheAbstractLaw
{
    static AboveTheLaw()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(AboveTheLaw));
    }

    public override Func<CardModel, bool> Filter => (_ => true);
}