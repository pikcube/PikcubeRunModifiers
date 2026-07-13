using BaseLib.Config;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

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