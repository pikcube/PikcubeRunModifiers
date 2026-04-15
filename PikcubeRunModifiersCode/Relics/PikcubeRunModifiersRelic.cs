using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Relics
{
    public abstract class PikcubeRunModifiersRelic : CustomRelicModel
    {
        //PikcubeRunModifiers/images/relics
        public override string PackedIconPath
        {
            get
            {
                string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
                return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
            }
        }

        protected override string PackedIconOutlinePath
        {
            get
            {
                string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
                return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
            }
        }

        protected override string BigIconPath
        {
            get
            {
                string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
                return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
            }
        }
    }
}