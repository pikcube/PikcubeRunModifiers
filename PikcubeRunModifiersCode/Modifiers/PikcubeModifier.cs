using MegaCrit.Sts2.Core.Models;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public abstract class PikcubeModifier : ModifierModel
{
    protected override string IconPath => GetImagePath("packed/modifiers/" + Id.Entry.ToLowerInvariant() + ".png");

    public static string GetImagePath(string innerPath)
    {
        return $"res://{MainFile.ModId}/images/{innerPath.TrimStart('/')}";
    }
}