using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using PengfuNailongMod.PengfuNailongModCode.Cards.Basic;
using PengfuNailongMod.PengfuNailongModCode.Extensions;
using PengfuNailongMod.PengfuNailongModCode.Relics;

namespace PengfuNailongMod.PengfuNailongModCode.Character;

public sealed class PengfuNailong : PlaceholderCharacterModel
{
    public const string CharacterId = "PengfuNailong";
    public static readonly Color Color = new("f5be2b");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 74;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<NailongStrike>(),
        ModelDb.Card<NailongStrike>(),
        ModelDb.Card<NailongStrike>(),
        ModelDb.Card<NailongStrike>(),
        ModelDb.Card<NailongStrike>(),
        ModelDb.Card<NailongDefend>(),
        ModelDb.Card<NailongDefend>(),
        ModelDb.Card<NailongDefend>(),
        ModelDb.Card<NailongDefend>(),
        ModelDb.Card<EmotionalOutburst>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<NailongBelly>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<PengfuNailongCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<PengfuNailongRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<PengfuNailongPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            Control icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_nailong.png".CharacterUiPath();
    public override string? CustomCharacterSelectIconPath => "char_select_nailong.png".CharacterUiPath();
    public override string? CustomCharacterSelectLockedIconPath => "char_select_nailong_locked.png".CharacterUiPath();
    public override string? CustomMapMarkerPath => "map_marker_nailong.png".CharacterUiPath();
}
