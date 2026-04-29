using BaseLib.Abstracts;
using Godot;
using InkSeerMod.InkSeerModCode.Extensions;

namespace InkSeerMod.InkSeerModCode.Character;

public sealed class InkSeerRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => InkSeer.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
