using PengfuNailongMod.PengfuNailongModCode.Audio;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;

namespace PengfuNailongMod.PengfuNailongModCode.Visuals;

public static class NailongActionDirector
{
    public static void Play(NailongActionKind kind)
    {
        NailongAudio.PlayAction(kind);
        NailongRiggedMotion.React(kind);
    }

    public static void PlayCue(NailongActionKind kind)
    {
        NailongAudio.PlayAction(kind);
        NailongRiggedMotion.React(kind);
    }

    public static void PlayExpression(ExpressionKind kind)
    {
        NailongAudio.PlayExpression(kind);
        NailongRiggedMotion.React(kind switch
        {
            ExpressionKind.Laugh => NailongActionKind.ExpressionLaugh,
            ExpressionKind.Fright => NailongActionKind.ExpressionFright,
            ExpressionKind.Angry => NailongActionKind.ExpressionAngry,
            ExpressionKind.Smug => NailongActionKind.ExpressionSmug,
            ExpressionKind.Aggrieved => NailongActionKind.ExpressionAggrieved,
            _ => NailongActionKind.ClearExpression
        });
    }
}
