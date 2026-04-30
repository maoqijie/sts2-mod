using BaseLib.Abstracts;
using Godot;
using PengfuNailongMod.PengfuNailongModCode.Extensions;

namespace PengfuNailongMod.PengfuNailongModCode.Character;

public sealed class PengfuNailongCardPool : CustomCardPoolModel
{
    public override string Title => PengfuNailong.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 0.13f;
    public override float S => 0.82f;
    public override float V => 0.92f;

    public override Color DeckEntryCardColor => PengfuNailong.Color;
    public override bool IsColorless => false;
}
