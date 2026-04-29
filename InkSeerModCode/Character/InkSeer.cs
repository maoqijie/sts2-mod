using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using InkSeerMod.InkSeerModCode.Cards.Basic;
using InkSeerMod.InkSeerModCode.Character;
using InkSeerMod.InkSeerModCode.Extensions;
using InkSeerMod.InkSeerModCode.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace InkSeerMod.InkSeerModCode.Character;

public sealed class InkSeer : PlaceholderCharacterModel
{
    public const string CharacterId = "InkSeer";
    public static readonly Color Color = new("2f6f73");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 72;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<InkStrike>(),
        ModelDb.Card<InkStrike>(),
        ModelDb.Card<InkStrike>(),
        ModelDb.Card<InkStrike>(),
        ModelDb.Card<InkStrike>(),
        ModelDb.Card<InkDefend>(),
        ModelDb.Card<InkDefend>(),
        ModelDb.Card<InkDefend>(),
        ModelDb.Card<InkDefend>(),
        ModelDb.Card<OpeningDraft>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SealedInkstone>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<InkSeerCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<InkSeerRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<InkSeerPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            Control icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_ink_seer.png".CharacterUiPath();
    public override string? CustomCharacterSelectIconPath => "char_select_ink_seer.png".CharacterUiPath();
    public override string? CustomCharacterSelectLockedIconPath => "char_select_ink_seer_locked.png".CharacterUiPath();
    public override string? CustomMapMarkerPath => "map_marker_ink_seer.png".CharacterUiPath();
}
