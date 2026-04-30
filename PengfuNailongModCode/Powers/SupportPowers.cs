using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;

namespace PengfuNailongMod.PengfuNailongModCode.Powers;

public abstract class NailongPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;
}

public sealed class StubbornHitPower : NailongPower
{
    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        Player? player = Owner.Player;
        if (player == null || target != Owner || result.UnblockedDamage <= 0) return;
        if (ExpressionState.Current(player) != ExpressionKind.Smug) return;
        if (!ExpressionState.TryUseStubbornHit(player)) return;

        await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, cardSource);
    }
}

public sealed class RollingDefenseLinePower : NailongPower;

public sealed class EmotionKaleidoscopePower : NailongPower;

public sealed class HundredFacesPower : NailongPower;

public sealed class OutOfControlShowPower : NailongPower;

public sealed class FaceMasterPower : NailongPower;

public sealed class SoftReboundPower : NailongPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || dealer == null || result.UnblockedDamage <= 0) return;

        await CreatureCmd.Damage(choiceContext, dealer, result.UnblockedDamage, ValueProp.Unpowered, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public sealed class BounceAwayPower : NailongPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || dealer == null || result.UnblockedDamage <= 0) return;

        await CreatureCmd.Damage(choiceContext, dealer, Amount, ValueProp.Unpowered, Owner, null);
        await PowerCmd.Remove(this);
    }
}

public sealed class BigReboundPower : NailongPower
{
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || dealer == null || result.UnblockedDamage <= 0) return;

        await CreatureCmd.Damage(choiceContext, dealer, result.UnblockedDamage, ValueProp.Unpowered, Owner, null);
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
