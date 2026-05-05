using MegaCrit.Sts2.Core.DevConsole;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Players;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers.PikcubeRunModifiersCode.Commands;

public class RigLaw : AbstractConsoleCmd
{
    public override CmdResult Process(Player? issuingPlayer, string[] args)
    {
        if (args.Length > 0)
        {
            return new CmdResult(false, "No arguments are required");
        }

        if (issuingPlayer is null)
        {
            return new CmdResult(false, "Not in a run");
        }

        TheLaw? lawModifier = issuingPlayer.RunState.Modifiers.OfType<TheLaw>().SingleOrDefault();

        if (lawModifier is null)
        {
            return new CmdResult(false, "Not in a law run");
        }

        lawModifier.RigNext(issuingPlayer);

        return new CmdResult(true, "The next card reward generated will have your law card in it");
    }

    public override string CmdName => "pikcube.rig";
    public override string Args => "pikcube.rig";
    public override string Description => "Force your next generated card reward to contain your law card.";
    public override bool IsNetworked => true; //I don't know what this means, but it seems like a good way to avoid state divergance
}