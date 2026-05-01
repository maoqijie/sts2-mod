using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Relics;

public sealed class NailongBelly : NailongRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
}

public sealed class ChangingNailongBelly : NailongRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}

public sealed class BellyAmulet : NailongRelic
{
    private bool used;

    public override RelicRarity Rarity => RelicRarity.Common;

    public override Task BeforeCombatStart()
    {
        used = false;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (used || Owner == null || target != Owner.Creature || result.UnblockedDamage <= 0) return;
        if (!DamageFilters.IsAttackDamage(props)) return;

        used = true;
        NailongActionDirector.Play(NailongActionKind.BonusBlock);
        await CreatureCmd.GainBlock(Owner.Creature, 8, ValueProp.Unpowered, null, fast: true);
        await ExpressionState.Enter(choiceContext, null, Owner, ExpressionKind.Fright, isRandom: false);
    }
}

public sealed class ChaosButton : NailongRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}

public sealed class ReverseSticker : NailongRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
}
