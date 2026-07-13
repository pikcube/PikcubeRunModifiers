using BaseLib.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Modding;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace PikcubeRunModifiers;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "PikcubeRunModifiers"; //Used for resource filepath

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();

        ModConfigRegistry.Register(ModId, new MapConfigMenu());

        BetterHooks.AfterOneTimeInitialization += BetterHooks_AfterOneTimeInitialization;
    }

    private static void BetterHooks_AfterOneTimeInitialization()
    {
        _ = PikcubeRunModifierModel.ModifierMap;
    }
}

public class MapConfigMenu : SimpleModConfig
{
    [ConfigSection("Bugged Map Settings")]
    public static ListedMapPointType MonsterRooms { get; set; } = ListedMapPointType.Monster;
    public static ListedMapPointType EliteRooms { get; set; } = ListedMapPointType.Elite;
    public static ListedMapPointType CampfireRooms { get; set; } = ListedMapPointType.Campfire;
    public static ListedMapPointType TreasureRooms { get; set; } = ListedMapPointType.Treasure;
    public static ListedMapPointType ShopRooms { get; set; } = ListedMapPointType.Shop;
    public static ListedMapPointType UnknownRooms { get; set; } = ListedMapPointType.Unknown;
}

public static class ListedMapPointExtension
{
    public static MapPointType ToMapPoint(this ListedMapPointType point)
    {
        return point switch
        {
            ListedMapPointType.Unknown => MapPointType.Unknown,
            ListedMapPointType.Shop => MapPointType.Shop,
            ListedMapPointType.Treasure => MapPointType.Treasure,
            ListedMapPointType.Campfire => MapPointType.RestSite,
            ListedMapPointType.Monster => MapPointType.Monster,
            ListedMapPointType.Elite => MapPointType.Elite,
            _ => MapPointType.Unknown
        };
    }
}

public enum ListedMapPointType
{
    Unknown,
    Shop,
    Treasure,
    Campfire,
    Monster,
    Elite
}