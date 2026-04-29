using BaseLib.Utils;
using InkSeerMod.InkSeerModCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace InkSeerMod.InkSeerModCode.Cards.Basic;

public sealed class OpeningDraft : InkSeerCard
{
    public OpeningDraft() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(3);
        WithCards(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        await CommonActions.Draw(this, choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
}
