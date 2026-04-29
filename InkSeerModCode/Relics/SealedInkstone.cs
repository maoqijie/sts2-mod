using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace InkSeerMod.InkSeerModCode.Relics;

public sealed class SealedInkstone : InkSeerRelic
{
    private const string ExtraDamageKey = "ExtraDamage";

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(ExtraDamageKey, 1)
    ];

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!props.IsPoweredAttack_())
        {
            return 0m;
        }

        if (cardSource?.Owner != Owner)
        {
            return 0m;
        }

        return DynamicVars[ExtraDamageKey].BaseValue;
    }
}
