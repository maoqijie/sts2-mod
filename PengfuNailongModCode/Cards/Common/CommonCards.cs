using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using PengfuNailongMod.PengfuNailongModCode.Cards;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Cards.Common;

public sealed class BellyBounce : NailongCard
{
    public BellyBounce() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 3);
        WithExpressionKeywords(ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Attack(choiceContext, play);
        if (CurrentIs(ExpressionKind.Fright)) await GainBlock(3);
    }
}

public sealed class PengfuCombo : NailongCard
{
    public PengfuCombo() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(5, 2);
        WithExpressionKeywords(ExpressionKind.Laugh);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Attack(choiceContext, play, hitCount: 2);
        if (CurrentIs(ExpressionKind.Laugh)) await Draw(choiceContext, 1);
    }
}

public sealed class BellyCushion : NailongCard
{
    public BellyCushion() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
        WithExpressionKeywords(ExpressionKind.Laugh);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Block(play);
        if (CurrentIs(ExpressionKind.Laugh)) await Draw(choiceContext, 1);
    }
}

public sealed class CurlUp : NailongCard, IIgnoreExpressionBlockModifier
{
    public CurlUp() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithExpressionKeywords(ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Fright);
        await Block(play);
    }
}

public sealed class HardToHoldLaugh : NailongCard
{
    public HardToHoldLaugh() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithCards(1, 1);
        WithExpressionKeywords(ExpressionKind.Laugh);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, ExpressionKind.Laugh);
        await Draw(choiceContext, DynamicVars.Cards.BaseValue);
    }
}

public sealed class FaceChaos : NailongCard
{
    public FaceChaos() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomBasic(choiceContext);
    }
}

public sealed class BellyCharge : NailongCard
{
    public BellyCharge() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Attack(choiceContext, play, actionKind: NailongActionKind.HeavyAttack);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this || !ExpressionState.HasEnteredExpressionThisTurn(Owner)) return false;

        modifiedCost = Math.Max(0m, originalCost - 1m);
        return true;
    }
}

public sealed class StartledFlailingSlap : NailongCard, IIgnoreExpressionAttackModifier
{
    public StartledFlailingSlap() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 3);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomBasic(choiceContext);
        await Attack(choiceContext, play);
    }
}

public sealed class BellyHugCurl : NailongCard
{
    public BellyHugCurl() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) => WithBlock(5, 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        bool firstExpression = ExpressionState.IsFirstExpressionThisTurn(Owner);
        await Block(play);
        if (firstExpression) await GainBlock(4);
    }
}

public sealed class Overreaction : NailongCard, IIgnoreExpressionBlockModifier
{
    public Overreaction() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(4, 3);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Enter(choiceContext, CurrentIs(ExpressionKind.Laugh) ? ExpressionKind.Fright : ExpressionKind.Laugh);
        await Block(play);
    }
}

public sealed class BellyPad : NailongCard
{
    public BellyPad() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithExpressionKeywords(ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Block(play);
        if (CurrentIs(ExpressionKind.Fright)) await GainBlock(3);
    }
}

public sealed class SneezeFaceChange : NailongCard
{
    public SneezeFaceChange() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithExpressionKeywords(ExpressionKind.Laugh, ExpressionKind.Fright);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        ExpressionKind? before = ExpressionState.Current(Owner);
        await EnterRandomBasic(choiceContext);
        if (ExpressionState.Current(Owner) != before) await GainBlock(4);
    }
}

public sealed class LaughingEndure : NailongCard, IIgnoreExpressionBlockPenalty
{
    public LaughingEndure() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) => WithBlock(7, 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        bool penalty = CurrentBlockPenalty();
        await Block(play);
        if (penalty) await GainBlock(3);
    }
}

public sealed class ScaredSmack : NailongCard, IIgnoreExpressionAttackPenalty
{
    public ScaredSmack() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) => WithDamage(8, 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        bool penalty = CurrentAttackPenalty();
        await Attack(choiceContext, play);
        if (penalty) await GainBlock(3);
    }
}
