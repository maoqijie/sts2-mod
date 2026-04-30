namespace PengfuNailongMod.PengfuNailongModCode.Mechanics;

public static class ExpressionRules
{
    public static ExpressionKind[] BasicExpressions { get; } =
    [
        ExpressionKind.Laugh,
        ExpressionKind.Fright
    ];

    public static ExpressionKind[] AllExpressions { get; } =
    [
        ExpressionKind.Laugh,
        ExpressionKind.Fright,
        ExpressionKind.Angry,
        ExpressionKind.Smug,
        ExpressionKind.Aggrieved
    ];

    public static decimal AttackAdditive(ExpressionKind kind)
    {
        return kind switch
        {
            ExpressionKind.Laugh => 2m,
            ExpressionKind.Fright => -2m,
            ExpressionKind.Angry => 1m,
            ExpressionKind.Aggrieved => -3m,
            _ => 0m
        };
    }

    public static decimal BlockAdditive(ExpressionKind kind)
    {
        return kind switch
        {
            ExpressionKind.Laugh => -2m,
            ExpressionKind.Fright => 3m,
            ExpressionKind.Angry => -3m,
            _ => 0m
        };
    }

    public static bool IsAttackPenalty(ExpressionKind kind) => AttackAdditive(kind) < 0m;

    public static bool IsBlockPenalty(ExpressionKind kind) => BlockAdditive(kind) < 0m;
}
