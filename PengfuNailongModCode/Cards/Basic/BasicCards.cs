using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using PengfuNailongMod.PengfuNailongModCode.Cards;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;

namespace PengfuNailongMod.PengfuNailongModCode.Cards.Basic;

public sealed class NailongStrike : NailongCard
{
    public NailongStrike() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithTags(CardTag.Strike);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Attack(choiceContext, play);
    }
}

public sealed class NailongDefend : NailongCard
{
    public NailongDefend() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(5, 3);
        WithTags(CardTag.Defend);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await Block(play);
    }
}

public sealed class EmotionalOutburst : NailongCard
{
    public EmotionalOutburst() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(3, 2);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await EnterRandomBasic(choiceContext);
        await Block(play);
    }
}
