using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using PengfuNailongMod.PengfuNailongModCode;
using PengfuNailongMod.PengfuNailongModCode.Cards.Basic;
using PengfuNailongMod.PengfuNailongModCode.Extensions;
using PengfuNailongMod.PengfuNailongModCode.Relics;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

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
            Control frame = new()
            {
                ClipContents = true,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                CustomMinimumSize = Vector2.Zero
            };
            TextureRect icon = new()
            {
                Texture = ResourceLoader.Load<Texture2D>(CustomIconTexturePath),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                CustomMinimumSize = Vector2.Zero
            };

            frame.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            icon.OffsetLeft = 4.0f;
            icon.OffsetTop = 4.0f;
            icon.OffsetRight = -4.0f;
            icon.OffsetBottom = -4.0f;
            frame.AddChild(icon);
            return frame;
        }
    }

    public override string CustomIconTexturePath => "character_icon_nailong.png".CharacterUiPath();
    public override string CustomVisualPath => "character_model_nailong.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "scenes/combat/energy_counter_pengfu_nailong.tscn".ModResourcePath();
    public override string CustomCharacterSelectBg => "scenes/char_select/char_select_bg_pengfu_nailong.tscn".ModResourcePath();
    public override string CustomRestSiteAnimPath => "scenes/rest_site/rest_site_pengfu_nailong.tscn".ModResourcePath();
    public override string CustomMerchantAnimPath => "scenes/merchant/merchant_pengfu_nailong.tscn".ModResourcePath();
    public override string CustomArmPointingTexturePath => "hand_point_nailong.png".CharacterUiPath();
    public override string CustomArmRockTexturePath => "hand_rock_nailong.png".CharacterUiPath();
    public override string CustomArmPaperTexturePath => "hand_paper_nailong.png".CharacterUiPath();
    public override string CustomArmScissorsTexturePath => "hand_scissors_nailong.png".CharacterUiPath();
    public override string? CustomCharacterSelectIconPath => "char_select_nailong.png".CharacterUiPath();
    public override string? CustomCharacterSelectLockedIconPath => "char_select_nailong_locked.png".CharacterUiPath();
    public override string? CustomMapMarkerPath => "map_marker_nailong.png".CharacterUiPath();

    public override NCreatureVisuals? CreateCustomVisuals()
    {
        NCreatureVisuals visuals = NodeFactory<NCreatureVisuals>.CreateFromResource(CustomVisualPath);
        NailongRiggedMotion.Attach(visuals);
        return visuals;
    }
}
