using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using InkSeerMod.InkSeerModCode.Character;
using InkSeerMod.InkSeerModCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace InkSeerMod.InkSeerModCode.Cards;

[Pool(typeof(InkSeerCardPool))]
public abstract class InkSeerCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : ConstructedCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath
    {
        get
        {
            string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".BigCardImagePath();
        }
    }

    public override string PortraitPath
    {
        get
        {
            string path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : "card.png".CardImagePath();
        }
    }

    public override string BetaPortraitPath
    {
        get
        {
            string path = $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : PortraitPath;
        }
    }
}
