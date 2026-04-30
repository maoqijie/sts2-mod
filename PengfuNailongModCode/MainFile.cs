using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace PengfuNailongMod.PengfuNailongModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "PengfuNailongMod";
    public const string ResPath = "res://";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);
        harmony.PatchAll();
    }
}
