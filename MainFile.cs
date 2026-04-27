using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using Pikcube.Common.Utility;
using PikcubeRunModifiers.PikcubeRunModifiersCode.Modifiers;

namespace PikcubeRunModifiers
{
    [ModInitializer(nameof(Initialize))]
    public partial class MainFile : Node
    {
        public const string ModId = "PikcubeRunModifiers"; //Used for resource filepath

        public static RelicSpawnManager RelicSpawnManager { get; set; } = new();

        public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

        public static void Initialize()
        {
            Harmony harmony = new(ModId);

            harmony.PatchAll();

            CustomRunManager.Register<AlwaysWhale>(CustomRunType.Good);
            CustomRunManager.Register<Dig>(CustomRunType.Good);
            CustomRunManager.Register<GadgetsModifier>(CustomRunType.Good);
            CustomRunManager.Register<Heirloom>(CustomRunType.Good);
            CustomRunManager.Register<TheILoveVakuuModifier>(CustomRunType.Good);
            CustomRunManager.Register<Inception>(CustomRunType.Good);
            CustomRunManager.Register<MyUncleOwnsMegacrit>(CustomRunType.Good);
            CustomRunManager.Register<Neapolitan>(CustomRunType.Good);
            CustomRunManager.Register<MyTrueForm>(CustomRunType.Good);
            CustomRunManager.Register<PraiseSnecko>(CustomRunType.Good);
            CustomRunManager.Register<TexasHoldem>(CustomRunType.Good);
            CustomRunManager.Register<TheLaw>(CustomRunType.Good);
            //CustomRunManager.Register<AboveTheLaw>(CustomRunType.Good);

            CustomRunManager.Register<FourForFourModifier>(CustomRunType.Bad);
            CustomRunManager.Register<FiftyFifty>(CustomRunType.Bad);
            CustomRunManager.Register<BlahajEnjoyer>(CustomRunType.Bad);
            CustomRunManager.Register<CurseOfGreedModifier>(CustomRunType.Bad);
            CustomRunManager.Register<TheIGotARockModifier>(CustomRunType.Bad);
            //CustomRunManager.Register<OneOfEverything>(CustomRunType.Bad);
            CustomRunManager.Register<PeakGaming>(CustomRunType.Bad);
            CustomRunManager.Register<PerfectLethal>(CustomRunType.Bad);
            CustomRunManager.Register<StickyStartersModifier>(CustomRunType.Bad);
        }
    }
}
