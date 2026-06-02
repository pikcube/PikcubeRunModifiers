using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public abstract class PikcubeRunModifierModel : CustomModifierModel
{
    public override int SortOrder => IsCanonical ? ModifierMap[this] : ModifierMap[ModelDb.GetById<PikcubeRunModifierModel>(Id)];

    internal static Dictionary<PikcubeRunModifierModel, int> ModifierMap
    {
        get
        {
            field ??= Init();
            return field;
        }
    }

    private static Dictionary<PikcubeRunModifierModel, int> Init()
    {
        Dictionary<PikcubeRunModifierModel, string> dict = ModelDb
            .AllModels.OfType<PikcubeRunModifierModel>()
            .ToDictionary(prm => prm, prm => prm.Title.GetInvariantRawText());

        string[] values = [.. dict.Values.Order()];

        Dictionary<PikcubeRunModifierModel, int> indexDic = [];
        foreach (KeyValuePair<PikcubeRunModifierModel, string> pair in dict)
        {
            int index = values.IndexOf(dict[pair.Key]);
            indexDic.Add(pair.Key, index + 1000);
        }

        return indexDic;
    }

    protected override string IconPath => Path.Join(MainFile.ModId, "images", "modifiers", $"{Id.Entry.ToLowerInvariant()}.png");
}