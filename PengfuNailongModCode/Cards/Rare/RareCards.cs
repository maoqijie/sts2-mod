using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using PengfuNailongMod.PengfuNailongModCode.Cards;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Powers;

namespace PengfuNailongMod.PengfuNailongModCode.Cards.Rare;

public sealed class FiveEmotions : NailongCard
{
    public FiveEmotions() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) => WithKeywords(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        foreach (ExpressionKind kind in ExpressionRules.AllExpressions)
        {
            await Enter(choiceContext, kind);
        }
    }
}

public sealed class EmotionKaleidoscope : NailongCard
{
    public EmotionKaleidoscope() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) => WithPower<EmotionKaleidoscopePower>(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<EmotionKaleidoscopePower>(DynamicVars.Power<EmotionKaleidoscopePower>().BaseValue);
    }
}

public sealed class EmotionExplosion : NailongCard
{
    public EmotionExplosion() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(16, 6);
        WithVar("ExtraDamage", 5, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ArgumentNullException.ThrowIfNull(play.Target);
        decimal amount = DamageValue + Dyn("ExtraDamage") * ExpressionState.EnteredExpressionCount(Owner);
        await AttackAmount(choiceContext, play.Target, amount);
    }
}

public sealed class HundredFaces : NailongCard
{
    public HundredFaces() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) => WithPower<HundredFacesPower>(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<HundredFacesPower>(DynamicVars.Power<HundredFacesPower>().BaseValue);
    }
}

public sealed class ChangingExpressionPack : NailongCard
{
    public ChangingExpressionPack() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCards(2, 1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class OutOfControlPerformance : NailongCard
{
    public OutOfControlPerformance() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) => WithPower<OutOfControlShowPower>(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<OutOfControlShowPower>(DynamicVars.Power<OutOfControlShowPower>().BaseValue);
    }
}

public sealed class FaceMaster : NailongCard
{
    public FaceMaster() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self) => WithPower<FaceMasterPower>(1);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await ApplySelf<FaceMasterPower>(DynamicVars.Power<FaceMasterPower>().BaseValue);
    }
}

public sealed class ExpressionRoulette : NailongCard
{
    public ExpressionRoulette() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) => WithKeywords(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await EnterRandomAny(choiceContext);
        await EnterRandomAny(choiceContext);
    }
}

public sealed class HugeBellyRebound : NailongCard
{
    public HugeBellyRebound() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(12, 4);
        WithPower<BigReboundPower>(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Block(play);
        await ApplySelf<BigReboundPower>(DynamicVars.Power<BigReboundPower>().BaseValue);
    }
}

public sealed class CryToLaugh : NailongCard
{
    public CryToLaugh() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(8, 3);
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Aggrieved);
        await Block(play);
        await Enter(choiceContext, ExpressionKind.Laugh);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class EveryoneLaughs : NailongCard
{
    public EveryoneLaughs() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithPower<WeakPower>(2, 1);
        WithPower<VulnerablePower>(2, 1);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Laugh);
        foreach (var enemy in Owner.Creature.CombatState!.HittableEnemies)
        {
            await ApplyTarget<WeakPower>(enemy, DynamicVars.Power<WeakPower>().BaseValue);
            await ApplyTarget<VulnerablePower>(enemy, DynamicVars.Vulnerable.BaseValue);
        }
    }
}

public sealed class EmotionalOverload : NailongCard
{
    public EmotionalOverload() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self) => WithKeywords(CardKeyword.Exhaust);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomAny(choiceContext);
        await EnterRandomAny(choiceContext);
        await PlayerCmd.GainEnergy(ExpressionState.EnteredExpressionCount(Owner), Owner);
    }
}
