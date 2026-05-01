using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using PengfuNailongMod.PengfuNailongModCode.Character;
using PengfuNailongMod.PengfuNailongModCode.Extensions;
using PengfuNailongMod.PengfuNailongModCode.Keywords;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Powers;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Cards;

[Pool(typeof(PengfuNailongCardPool))]
public abstract class NailongCard(int cost, CardType type, CardRarity rarity, TargetType target)
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

    protected Task Attack(PlayerChoiceContext context, CardPlay play, int hitCount = 1, NailongActionKind? actionKind = null)
    {
        NailongActionDirector.Play(actionKind ?? (hitCount > 1 ? NailongActionKind.MultiHitAttack : NailongActionKind.Attack));
        return CommonActions.CardAttack(this, play, hitCount).Execute(context);
    }

    protected Task AttackAmount(PlayerChoiceContext context, Creature target, decimal amount)
    {
        NailongActionDirector.Play(NailongActionKind.HeavyAttack);
        return CommonActions.CardAttack(this, target, amount, 1).Execute(context);
    }

    protected Task Block(CardPlay play, NailongActionKind actionKind = NailongActionKind.Block)
    {
        NailongActionDirector.Play(actionKind);
        return CommonActions.CardBlock(this, play);
    }

    protected async Task GainBlock(decimal amount)
    {
        NailongActionDirector.Play(NailongActionKind.BonusBlock);
        await CreatureCmd.GainBlock(Owner.Creature, amount, ValueProp.Unpowered, null, fast: true);
    }

    protected Task Draw(PlayerChoiceContext context, decimal count)
    {
        NailongActionDirector.Play(NailongActionKind.Draw);
        return CardPileCmd.Draw(context, count, Owner);
    }

    protected Task Enter(PlayerChoiceContext context, ExpressionKind kind, bool isRandom = false)
    {
        return ExpressionState.Enter(context, this, Owner, kind, isRandom);
    }

    protected Task EnterRandomBasic(PlayerChoiceContext context)
    {
        return Enter(context, ExpressionState.RandomBasic(), isRandom: true);
    }

    protected Task EnterRandomAny(PlayerChoiceContext context)
    {
        return Enter(context, ExpressionState.RandomAny(Owner), isRandom: true);
    }

    protected Task ApplySelf<T>(decimal amount) where T : PowerModel
    {
        NailongActionDirector.Play(NailongActionKind.Power);
        return PowerCmd.Apply<T>(Owner.Creature, amount, Owner.Creature, this);
    }

    protected Task ApplyTarget<T>(Creature target, decimal amount, bool playCue = true) where T : PowerModel
    {
        if (playCue)
        {
            NailongActionDirector.Play(NailongActionKind.Power);
        }

        return PowerCmd.Apply<T>(target, amount, Owner.Creature, this);
    }

    protected async Task GainEnergy(decimal amount)
    {
        NailongActionDirector.Play(NailongActionKind.Energy);
        await PlayerCmd.GainEnergy(amount, Owner);
    }

    protected void WithExpressionKeywords(params ExpressionKind[] kinds)
    {
        WithKeywords(kinds.Select(ExpressionKeywords.For).ToArray());
    }

    protected bool CurrentIs(ExpressionKind kind) => ExpressionState.Current(Owner) == kind;

    protected bool CurrentAttackPenalty()
    {
        ExpressionKind? kind = ExpressionState.Current(Owner);
        return kind.HasValue && ExpressionRules.IsAttackPenalty(kind.Value);
    }

    protected bool CurrentBlockPenalty()
    {
        ExpressionKind? kind = ExpressionState.Current(Owner);
        return kind.HasValue && ExpressionRules.IsBlockPenalty(kind.Value);
    }

    protected decimal Dyn(string name) => DynamicVars[name].BaseValue;

    protected decimal DamageValue => DynamicVars.Damage.BaseValue;
    protected decimal BlockValue => DynamicVars.Block.BaseValue;
}
