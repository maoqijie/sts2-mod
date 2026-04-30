using BaseLib.Abstracts;
using Godot;
using PengfuNailongMod.PengfuNailongModCode.Extensions;

namespace PengfuNailongMod.PengfuNailongModCode.Character;

public sealed class PengfuNailongRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => PengfuNailong.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
