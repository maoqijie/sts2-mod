using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using PengfuNailongMod.PengfuNailongModCode.Powers;
using PengfuNailongMod.PengfuNailongModCode.Relics;

namespace PengfuNailongMod.PengfuNailongModCode.Mechanics;

public static class ExpressionState
{
    private static readonly Dictionary<Player, PlayerExpressionState> States = new();

    public static ExpressionKind? Current(Player player) => Get(player).Current;

    public static bool HasEnteredExpressionThisTurn(Player player) => Get(player).EnteredThisTurn;

    public static bool HasRandomEnteredExpressionThisTurn(Player player) => Get(player).RandomEnteredThisTurn;

    public static int EnteredExpressionCount(Player player) => Get(player).EnteredThisCombat.Count;

    public static bool HasEnteredThisCombat(Player player, ExpressionKind kind) => Get(player).EnteredThisCombat.Contains(kind);

    public static bool IsFirstExpressionThisTurn(Player player) => !Get(player).EnteredThisTurn;

    public static bool IsFirstTimeExpressionThisCombat(Player player, ExpressionKind kind)
    {
        return !Get(player).EnteredThisCombat.Contains(kind);
    }

    public static ExpressionKind RandomBasic() => Pick(ExpressionRules.BasicExpressions);

    public static ExpressionKind RandomAny() => Pick(ExpressionRules.AllExpressions);

    public static ExpressionKind RandomAny(Player player)
    {
        if (!player.Creature.HasPower<FaceMasterPower>())
        {
            return RandomAny();
        }

        List<ExpressionKind> unseen = ExpressionRules.AllExpressions
            .Where(kind => !HasEnteredThisCombat(player, kind))
            .ToList();
        return unseen.Count > 0 ? Pick(unseen) : RandomAny();
    }

    public static Task BeginTurn(Player player)
    {
        PlayerExpressionState state = Get(player);
        state.EnteredThisTurn = false;
        state.RandomEnteredThisTurn = false;
        state.SmugDrawUsed = false;
        state.AggrievedBlockUsed = false;
        state.StubbornHitUsed = false;
        state.ReverseStickerUsed = false;
        return Task.CompletedTask;
    }

    public static async Task Enter(PlayerChoiceContext context, CardModel? source, Player player, ExpressionKind kind, bool isRandom)
    {
        PlayerExpressionState state = Get(player);
        bool firstThisTurn = !state.EnteredThisTurn;
        bool firstThisCombat = !state.EnteredThisCombat.Contains(kind);

        state.Current = kind;
        state.EnteredThisTurn = true;
        state.RandomEnteredThisTurn |= isRandom;
        state.EnteredThisCombat.Add(kind);

        await ReplaceExpressionPower(player, kind);
        await TriggerExpressionSideEffects(context, source, player, kind, isRandom, firstThisTurn, firstThisCombat);
    }

    public static async Task Clear(Player player)
    {
        PlayerExpressionState state = Get(player);
        state.Current = null;
        await PowerCmd.Remove<LaughExpressionPower>(player.Creature);
        await PowerCmd.Remove<FrightExpressionPower>(player.Creature);
        await PowerCmd.Remove<AngryExpressionPower>(player.Creature);
        await PowerCmd.Remove<SmugExpressionPower>(player.Creature);
        await PowerCmd.Remove<AggrievedExpressionPower>(player.Creature);
    }

    public static bool TryUseSmugDraw(Player player)
    {
        PlayerExpressionState state = Get(player);
        if (state.SmugDrawUsed) return false;

        state.SmugDrawUsed = true;
        return true;
    }

    public static bool TryUseAggrievedBlock(Player player)
    {
        PlayerExpressionState state = Get(player);
        if (state.AggrievedBlockUsed) return false;

        state.AggrievedBlockUsed = true;
        return true;
    }

    public static bool TryUseStubbornHit(Player player)
    {
        PlayerExpressionState state = Get(player);
        if (state.StubbornHitUsed) return false;

        state.StubbornHitUsed = true;
        return true;
    }

    public static bool TryUseReverseSticker(Player player, CardModel? cardSource)
    {
        if (!HasUsableRelic<ReverseSticker>(player)) return false;
        if (cardSource?.Pile?.Type != PileType.Play) return false;

        PlayerExpressionState state = Get(player);
        if (state.ReverseStickerUsed) return false;

        state.ReverseStickerUsed = true;
        return true;
    }

    private static async Task TriggerExpressionSideEffects(
        PlayerChoiceContext context,
        CardModel? source,
        Player player,
        ExpressionKind kind,
        bool isRandom,
        bool firstThisTurn,
        bool firstThisCombat)
    {
        if (firstThisTurn && (HasUsableRelic<NailongBelly>(player) || HasUsableRelic<ChangingNailongBelly>(player)))
        {
            await GainBlock(player, 3);
        }

        if (firstThisTurn && player.Creature.HasPower<RollingDefenseLinePower>())
        {
            await GainBlock(player, kind is ExpressionKind.Fright or ExpressionKind.Aggrieved ? 4 : 2);
        }

        if (firstThisCombat && HasUsableRelic<ChangingNailongBelly>(player))
        {
            await PlayerCmd.GainEnergy(1, player);
        }

        if (firstThisCombat && player.Creature.HasPower<EmotionKaleidoscopePower>())
        {
            await CardPileCmd.Draw(context, 1, player);
        }

        if (firstThisTurn && firstThisCombat && player.Creature.HasPower<HundredFacesPower>())
        {
            await PlayerCmd.GainEnergy(1, player);
        }

        if (isRandom && player.Creature.HasPower<OutOfControlShowPower>())
        {
            await PowerCmd.Apply<MegaCrit.Sts2.Core.Models.Powers.StrengthPower>(player.Creature, 1, player.Creature, source);
        }

        PlayerExpressionState state = Get(player);
        if (isRandom && HasUsableRelic<ChaosButton>(player) && !state.ChaosButtonUsed)
        {
            state.ChaosButtonUsed = true;
            await PlayerCmd.GainEnergy(1, player);
            await Enter(context, source, player, RandomBasic(), isRandom: false);
        }
    }

    private static async Task ReplaceExpressionPower(Player player, ExpressionKind kind)
    {
        await PowerCmd.Remove<LaughExpressionPower>(player.Creature);
        await PowerCmd.Remove<FrightExpressionPower>(player.Creature);
        await PowerCmd.Remove<AngryExpressionPower>(player.Creature);
        await PowerCmd.Remove<SmugExpressionPower>(player.Creature);
        await PowerCmd.Remove<AggrievedExpressionPower>(player.Creature);

        switch (kind)
        {
            case ExpressionKind.Laugh:
                await PowerCmd.Apply<LaughExpressionPower>(player.Creature, 1, player.Creature, null);
                break;
            case ExpressionKind.Fright:
                await PowerCmd.Apply<FrightExpressionPower>(player.Creature, 1, player.Creature, null);
                break;
            case ExpressionKind.Angry:
                await PowerCmd.Apply<AngryExpressionPower>(player.Creature, 1, player.Creature, null);
                break;
            case ExpressionKind.Smug:
                await PowerCmd.Apply<SmugExpressionPower>(player.Creature, 1, player.Creature, null);
                break;
            case ExpressionKind.Aggrieved:
                await PowerCmd.Apply<AggrievedExpressionPower>(player.Creature, 1, player.Creature, null);
                break;
        }
    }

    private static async Task GainBlock(Player player, decimal amount)
    {
        await CreatureCmd.GainBlock(player.Creature, amount, ValueProp.Unpowered, null, fast: true);
    }

    private static bool HasUsableRelic<T>(Player player) where T : RelicModel
    {
        return player.Relics.Any(relic => relic is T && !relic.IsMelted);
    }

    private static PlayerExpressionState Get(Player player)
    {
        if (!States.TryGetValue(player, out PlayerExpressionState? state))
        {
            state = new PlayerExpressionState();
            States[player] = state;
        }

        CombatState? combat = player.Creature.CombatState;
        int round = combat?.RoundNumber ?? 0;
        if (!ReferenceEquals(state.Combat, combat))
        {
            state.ResetCombat(combat, round);
        }
        else if (state.RoundNumber != round)
        {
            state.ResetTurn(round);
        }

        return state;
    }

    private static ExpressionKind Pick(IReadOnlyList<ExpressionKind> expressions)
    {
        return expressions[Random.Shared.Next(expressions.Count)];
    }

    private sealed class PlayerExpressionState
    {
        public CombatState? Combat { get; private set; }
        public int RoundNumber { get; private set; }
        public ExpressionKind? Current { get; set; }
        public bool EnteredThisTurn { get; set; }
        public bool RandomEnteredThisTurn { get; set; }
        public bool SmugDrawUsed { get; set; }
        public bool AggrievedBlockUsed { get; set; }
        public bool StubbornHitUsed { get; set; }
        public bool ReverseStickerUsed { get; set; }
        public bool ChaosButtonUsed { get; set; }
        public HashSet<ExpressionKind> EnteredThisCombat { get; } = [];

        public void ResetCombat(CombatState? combat, int round)
        {
            Combat = combat;
            Current = null;
            ChaosButtonUsed = false;
            EnteredThisCombat.Clear();
            ResetTurn(round);
        }

        public void ResetTurn(int round)
        {
            RoundNumber = round;
            EnteredThisTurn = false;
            RandomEnteredThisTurn = false;
            SmugDrawUsed = false;
            AggrievedBlockUsed = false;
            StubbornHitUsed = false;
            ReverseStickerUsed = false;
        }
    }
}
