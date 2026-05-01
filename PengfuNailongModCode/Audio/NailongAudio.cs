using Godot;
using PengfuNailongMod.PengfuNailongModCode.Mechanics;
using PengfuNailongMod.PengfuNailongModCode.Visuals;

namespace PengfuNailongMod.PengfuNailongModCode.Audio;

public static class NailongAudio
{
    private const ulong ExpressionCooldownMs = 260;
    private const ulong ActionCooldownMs = 140;
    private const float ExpressionVolumeDb = -7f;
    private const float ActionVolumeDb = -10f;
    private const float BasicCardVolumeDb = -3f;

    private static readonly Dictionary<ExpressionKind, string> ExpressionSoundPaths = new()
    {
        [ExpressionKind.Laugh] = MainFile.ResPath + "/audio/nailong/expression_laugh.ogg",
        [ExpressionKind.Fright] = MainFile.ResPath + "/audio/nailong/expression_fright.ogg",
        [ExpressionKind.Angry] = MainFile.ResPath + "/audio/nailong/expression_angry.ogg",
        [ExpressionKind.Smug] = MainFile.ResPath + "/audio/nailong/expression_smug.ogg",
        [ExpressionKind.Aggrieved] = MainFile.ResPath + "/audio/nailong/expression_aggrieved.ogg"
    };

    private static readonly Dictionary<NailongActionKind, string> ActionSoundPaths = new()
    {
        [NailongActionKind.NailongStrike] = MainFile.ResPath + "/audio/nailong/expression_laugh.ogg",
        [NailongActionKind.NailongDefend] = MainFile.ResPath + "/audio/nailong/expression_laugh.ogg",
        [NailongActionKind.Attack] = MainFile.ResPath + "/audio/nailong/action_attack.ogg",
        [NailongActionKind.MultiHitAttack] = MainFile.ResPath + "/audio/nailong/action_attack.ogg",
        [NailongActionKind.HeavyAttack] = MainFile.ResPath + "/audio/nailong/action_heavy.ogg",
        [NailongActionKind.Block] = MainFile.ResPath + "/audio/nailong/action_block.ogg",
        [NailongActionKind.BonusBlock] = MainFile.ResPath + "/audio/nailong/action_block.ogg",
        [NailongActionKind.Draw] = MainFile.ResPath + "/audio/nailong/action_draw.ogg",
        [NailongActionKind.Energy] = MainFile.ResPath + "/audio/nailong/action_energy.ogg",
        [NailongActionKind.Power] = MainFile.ResPath + "/audio/nailong/action_power.ogg",
        [NailongActionKind.ExpressionShift] = MainFile.ResPath + "/audio/nailong/action_power.ogg",
        [NailongActionKind.RandomExpression] = MainFile.ResPath + "/audio/nailong/action_energy.ogg",
        [NailongActionKind.ClearExpression] = MainFile.ResPath + "/audio/nailong/expression_clear.ogg"
    };

    private static readonly Dictionary<ExpressionKind, AudioClip> ExpressionStreams = [];
    private static readonly Dictionary<NailongActionKind, AudioClip> ActionStreams = [];
    private static readonly Dictionary<string, ulong> LastPlayedAtByClip = [];
    private static bool initialized;

    private readonly record struct AudioClip(AudioStream Stream, string CooldownKey);

    public static void Initialize()
    {
        if (initialized) return;

        initialized = true;
        foreach ((ExpressionKind kind, string path) in ExpressionSoundPaths)
        {
            Load(ExpressionStreams, kind, path);
        }

        foreach ((NailongActionKind kind, string path) in ActionSoundPaths)
        {
            Load(ActionStreams, kind, path);
        }
    }

    public static void PlayExpression(ExpressionKind kind)
    {
        if (!initialized) Initialize();
        if (!ExpressionStreams.TryGetValue(kind, out AudioClip clip)) return;

        PlayStream(clip, ExpressionCooldownMs, ExpressionVolumeDb);
    }

    public static void PlayAction(NailongActionKind kind)
    {
        if (!initialized) Initialize();
        if (!ActionStreams.TryGetValue(kind, out AudioClip clip)) return;

        PlayStream(clip, ActionCooldownMs, VolumeFor(kind));
    }

    private static float VolumeFor(NailongActionKind kind)
    {
        return kind is NailongActionKind.NailongStrike or NailongActionKind.NailongDefend
            ? BasicCardVolumeDb
            : ActionVolumeDb;
    }

    private static void PlayStream(
        AudioClip clip,
        ulong cooldownMs,
        float volumeDb)
    {
        ulong now = Time.GetTicksMsec();
        if (LastPlayedAtByClip.TryGetValue(clip.CooldownKey, out ulong lastPlayed) && now - lastPlayed < cooldownMs) return;

        if (Engine.GetMainLoop() is not SceneTree tree)
        {
            return;
        }

        AudioStreamPlayer player = new()
        {
            Bus = ResolveAudioBus(),
            Stream = clip.Stream,
            VolumeDb = volumeDb
        };

        player.Finished += player.QueueFree;
        tree.Root.AddChild(player);
        player.Play();
        LastPlayedAtByClip[clip.CooldownKey] = now;
    }

    private static void Load<TKey>(Dictionary<TKey, AudioClip> streams, TKey key, string path) where TKey : notnull
    {
        AudioStream? stream = ResourceLoader.Exists(path)
            ? ResourceLoader.Load<AudioStream>(path)
            : AudioStreamOggVorbis.LoadFromFile(path);

        if (stream == null)
        {
            MainFile.Logger.Info("Could not load Nailong audio path: " + path);
            return;
        }

        streams[key] = new AudioClip(stream, path);
    }

    private static string ResolveAudioBus()
    {
        return AudioServer.GetBusIndex("SFX") >= 0 ? "SFX" : "Master";
    }
}
