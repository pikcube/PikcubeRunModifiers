using MegaCrit.Sts2.Core.Map;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Utility;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

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