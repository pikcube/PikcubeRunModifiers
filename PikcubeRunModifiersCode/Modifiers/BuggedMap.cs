using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

public class BuggedMap : PikcubeRunModifierModel
{
    public override ModifierAlignment Alignment => ModifierAlignment.Good;

    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        foreach (MapPoint point in map.GetAllMapPoints().ToArray())
        {
            switch (point.PointType)
            {
                case MapPointType.Unknown:
                    point.PointType = MapConfigMenu.UnknownRooms.ToMapPoint();
                    break;
                case MapPointType.Shop:
                    point.PointType = MapConfigMenu.ShopRooms.ToMapPoint();
                    break;
                case MapPointType.Treasure:
                    point.PointType = MapConfigMenu.TreasureRooms.ToMapPoint();
                    break;
                case MapPointType.RestSite:
                    point.PointType = MapConfigMenu.CampfireRooms.ToMapPoint();
                    break;
                case MapPointType.Monster:
                    point.PointType = MapConfigMenu.MonsterRooms.ToMapPoint();
                    break;
                case MapPointType.Elite:
                    point.PointType = MapConfigMenu.EliteRooms.ToMapPoint();
                    break;
                case MapPointType.Unassigned:
            case MapPointType.Boss:
                case MapPointType.Ancient:
                default:
                    break;
            }
        }


        return map;
    }
}