using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using PengfuNailongMod.PengfuNailongModCode.Cards;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Powers;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Cards.Uncommon;

public sealed class AngryPuffedCheeks : NailongCard, IIgnoreExpressionBlockModifier
{
    public AngryPuffedCheeks() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 3);
        WithExpressionKeywords(ExpressionKind.Angry);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Angry);
        await Block(play);
    }
}

public sealed class AngryBellySlap : NailongCard, IIgnoreExpressionAttackModifier
{
    public AngryBellySlap() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(10, 4);
        WithExpressionKeywords(ExpressionKind.Angry);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Angry);
        await Attack(choiceContext, play, actionKind: NailongActionKind.HeavyAttack);
    }
}

public sealed class SmugHandsOnHips : NailongCard
{
    public SmugHandsOnHips() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(1, 1);
        WithExpressionKeywords(ExpressionKind.Smug);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Smug);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class OverconfidentDraw : NailongCard
{
    public OverconfidentDraw() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2, 1);
        WithExpressionKeywords(ExpressionKind.Smug);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Smug);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class TearyBlock : NailongCard
{
    public TearyBlock() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(9, 3);
        WithExpressionKeywords(ExpressionKind.Aggrieved);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Aggrieved);
        await Block(play);
    }
}

public sealed class TearyWeakAura : NailongCard
{
    public TearyWeakAura() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithPower<WeakPower>(2, 1);
        WithExpressionKeywords(ExpressionKind.Aggrieved);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Aggrieved);
        NailongActionDirector.Play(NailongActionKind.Power);
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await ApplyTarget<WeakPower>(enemy, DynamicVars.Power<WeakPower>().BaseValue, playCue: false);
        }
    }
}

public sealed class EmotionalBreakdown : NailongCard, IIgnoreExpressionAttackModifier
{
    public EmotionalBreakdown() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) => WithDamage(7, 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await Attack(choiceContext, play);
    }
}

public sealed class ExpressionMasks : NailongCard
{
    public ExpressionMasks() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) => WithKeywords(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await EnterRandomAny(choiceContext);
    }
}

public sealed class DeepBreath : NailongCard, IIgnoreExpressionBlockModifier
{
    public DeepBreath() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 3);
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ExpressionState.Clear(Owner);
        await Block(play);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class LaughingCounter : NailongCard
{
    public LaughingCounter() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Smug);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Attack(choiceContext, play);
        if (CurrentIs(ExpressionKind.Laugh) || CurrentIs(ExpressionKind.Smug)) await Draw(choiceContext, 1);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this || (!CurrentIs(ExpressionKind.Laugh) && !CurrentIs(ExpressionKind.Smug))) return false;

        modifiedCost = 0m;
        return true;
    }
}

public sealed class FrightenedBounceAway : NailongCard, IIgnoreExpressionBlockModifier
{
    public FrightenedBounceAway() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(6, 3);
        WithPower<BounceAwayPower>(4, 2);
        WithExpressionKeywords(ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Fright);
        await Block(play);
        await ApplySelf<BounceAwayPower>(DynamicVars.Power<BounceAwayPower>().BaseValue);
    }
}

public sealed class BellyChargeEnergy : NailongCard
{
    public BellyChargeEnergy() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) => WithEnergy(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await GainEnergy(DynamicVars.Energy.BaseValue);
    }
}

public sealed class SoftRebound : NailongCard
{
    public SoftRebound() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 3);
        WithPower<SoftReboundPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Block(play);
        await ApplySelf<SoftReboundPower>(DynamicVars.Power<SoftReboundPower>().BaseValue);
    }
}

public sealed class RollingDefenseLine : NailongCard
{
    public RollingDefenseLine() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<RollingDefenseLinePower>(1);
        WithExpressionKeywords(ExpressionKind.Fright, ExpressionKind.Aggrieved);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<RollingDefenseLinePower>(DynamicVars.Power<RollingDefenseLinePower>().BaseValue);
    }
}

public sealed class UncontrolledFace : NailongCard
{
    public UncontrolledFace() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) => WithKeywords(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ExpressionKind next = ExpressionState.RandomAny(Owner);
        bool seen = ExpressionState.HasEnteredThisCombat(Owner, next);
        await Enter(choiceContext, next, isRandom: true);
        if (seen) await Draw(choiceContext, 1);
    }
}

public sealed class LaughOrFrightAttack : NailongCard, IIgnoreExpressionAttackModifier
{
    public LaughOrFrightAttack() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomBasic(choiceContext);
        await Attack(choiceContext, play);
    }
}

public sealed class SmugHitStrength : NailongCard
{
    public SmugHitStrength() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<StubbornHitPower>(1);
        WithExpressionKeywords(ExpressionKind.Smug);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<StubbornHitPower>(DynamicVars.Power<StubbornHitPower>().BaseValue);
    }
}
