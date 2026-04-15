using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Extensions;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Powers
{
    public abstract class PikcubeRunModifiersPower : CustomPowerModel
    {
        //Loads from PikcubeRunModifiers/images/powers/your_power.png
        public override string CustomPackedIconPath
        {
            get
            {
                string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
                return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
            }
        }

        public override string CustomBigIconPath
        {
            get
            {
                string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
                return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
            }
        }
    }
}