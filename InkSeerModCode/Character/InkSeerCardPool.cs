using BaseLib.Abstracts;
using Godot;
using InkSeerMod.InkSeerModCode.Extensions;

namespace InkSeerMod.InkSeerModCode.Character;

public sealed class InkSeerCardPool : CustomCardPoolModel
{
    public override string Title => InkSeer.CharacterId;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    public override float H => 0.52f;
    public override float S => 0.78f;
    public override float V => 0.58f;

    public override Color DeckEntryCardColor => InkSeer.Color;
    public override bool IsColorless => false;
}
