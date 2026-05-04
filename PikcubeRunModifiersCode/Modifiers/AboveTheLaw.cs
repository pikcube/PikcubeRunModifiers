using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class AboveTheLaw (): TheAbstractLaw(CustomRunType.None, "Above the Law")
{
    static AboveTheLaw()
    {
        SavedPropertiesTypeCache.InjectTypeIntoCache(typeof(AboveTheLaw));
    }

    public override Func<CardModel, bool> Filter => (_ => true);
}