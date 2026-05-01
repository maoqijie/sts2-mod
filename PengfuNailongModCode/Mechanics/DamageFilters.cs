using BaseLib.Extensions;
using MegaCrit.Sts2.Core.ValueProps;

namespace PengfuNailongMod.PengfuNailongModCode.Mechanics;

public static class DamageFilters
{
    public static bool IsAttackDamage(ValueProp props) => props.IsPoweredAttack_();
}
