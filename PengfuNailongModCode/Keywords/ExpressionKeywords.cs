using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;

namespace PengfuNailongMod.PengfuNailongModCode.Keywords;

#pragma warning disable CA2211

public static class ExpressionKeywords
{
    [CustomEnum("Laugh")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Laugh;

    [CustomEnum("Fright")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Fright;

    [CustomEnum("Angry")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Angry;

    [CustomEnum("Smug")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Smug;

    [CustomEnum("Aggrieved")]
    [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Aggrieved;

    public static CardKeyword For(ExpressionKind kind)
    {
        return kind switch
        {
            ExpressionKind.Laugh => Laugh,
            ExpressionKind.Fright => Fright,
            ExpressionKind.Angry => Angry,
            ExpressionKind.Smug => Smug,
            ExpressionKind.Aggrieved => Aggrieved,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
    }
}
