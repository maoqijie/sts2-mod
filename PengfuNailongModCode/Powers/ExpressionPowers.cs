using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using PengfuNailongMod.PengfuNailongModCode.Extensions;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Powers;

public abstract class ExpressionPowerBase : CustomPowerModel
{
    protected abstract ExpressionKind Kind { get; }
    protected abstract string IconFileName { get; }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
    public override string? CustomPackedIconPath => $"{IconFileName}.png".PowerImagePath();
    public override string? CustomBigIconPath => $"{IconFileName}.png".PowerImagePath();

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!DamageFilters.IsAttackDamage(props)
            || dealer != Owner
            || cardSource?.Owner?.Creature != Owner
            || cardSource.Type != CardType.Attack)
        {
            return 0m;
        }

        if (cardSource is IIgnoreExpressionAttackModifier) return 0m;

        decimal modifier = ExpressionRules.AttackAdditive(Kind);
        if (modifier >= 0m) return modifier;
        if (cardSource is IIgnoreExpressionAttackPenalty) return 0m;

        Player? player = Owner.Player;
        return player != null && ExpressionState.TryUseReverseSticker(player, cardSource) ? 0m : modifier;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (target != Owner || cardSource?.Owner?.Creature != Owner || cardSource.Type != CardType.Skill)
        {
            return 0m;
        }

        if (cardSource is IIgnoreExpressionBlockModifier) return 0m;

        decimal modifier = ExpressionRules.BlockAdditive(Kind);
        if (modifier >= 0m) return modifier;
        if (cardSource is IIgnoreExpressionBlockPenalty) return 0m;

        Player? player = Owner.Player;
        return player != null && ExpressionState.TryUseReverseSticker(player, cardSource) ? 0m : modifier;
    }

    public override decimal ModifyHpLostBeforeOsty(
        Creature target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Kind == ExpressionKind.Smug && target == Owner && DamageFilters.IsAttackDamage(props) && amount > 0m)
        {
            return amount + 1m;
        }

        return amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            await ExpressionState.BeginTurn(player);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        Player? player = Owner.Player;
        if (player == null || cardPlay.Card.Owner != player) return;

        if (Kind == ExpressionKind.Smug && ExpressionState.TryUseSmugDraw(player))
        {
            NailongActionDirector.Play(NailongActionKind.Draw);
            await CardPileCmd.Draw(context, 1, player);
        }

        if (Kind == ExpressionKind.Angry && cardPlay.Card.Type == CardType.Attack && cardPlay.Target != null)
        {
            NailongActionDirector.Play(NailongActionKind.Power);
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, 1, Owner, cardPlay.Card);
        }
    }

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        Player? player = Owner.Player;
        if (Kind != ExpressionKind.Aggrieved || player == null || creature != Owner || amount <= 0m) return;
        if (!ExpressionState.TryUseAggrievedBlock(player)) return;

        NailongActionDirector.Play(NailongActionKind.BonusBlock);
        await CreatureCmd.GainBlock(Owner, 4, ValueProp.Unpowered, null, fast: true);
    }
}

public sealed class LaughExpressionPower : ExpressionPowerBase
{
    protected override ExpressionKind Kind => ExpressionKind.Laugh;
    protected override string IconFileName => "expression_laugh";
}

public sealed class FrightExpressionPower : ExpressionPowerBase
{
    protected override ExpressionKind Kind => ExpressionKind.Fright;
    protected override string IconFileName => "expression_fright";
}

public sealed class AngryExpressionPower : ExpressionPowerBase
{
    protected override ExpressionKind Kind => ExpressionKind.Angry;
    protected override string IconFileName => "expression_angry";
}

public sealed class SmugExpressionPower : ExpressionPowerBase
{
    protected override ExpressionKind Kind => ExpressionKind.Smug;
    protected override string IconFileName => "expression_smug";
}

public sealed class AggrievedExpressionPower : ExpressionPowerBase
{
    protected override ExpressionKind Kind => ExpressionKind.Aggrieved;
    protected override string IconFileName => "expression_aggrieved";
}
