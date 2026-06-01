

using BaseLib.Abstracts;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public abstract class PikcubeRunModifierModel : CustomModifierModel
{
    protected override string IconPath => Path.Join(MainFile.ModId, "images", "modifiers", $"{Id.Entry.ToLowerInvariant()}.png");
}