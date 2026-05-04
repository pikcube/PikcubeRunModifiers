using Pikcube.Common.Abstracts;
using Pikcube.Common.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public abstract class PikcubeRunModifierModel : CustomRunModifierModel
{
    protected PikcubeRunModifierModel(CustomRunType type, string modifierName) : base(type, new CustomRunModifierInfo(MainFile.ModId, modifierName))
    {
    }

    protected PikcubeRunModifierModel(CustomRunType type, CustomRunModifierInfo info) : base(type, info)
    {
    }

    public override string GetImagePath(string innerPath)
    {
        return $"res://{MainFile.ModId}/images/{innerPath.TrimStart('/')}";
    }
}