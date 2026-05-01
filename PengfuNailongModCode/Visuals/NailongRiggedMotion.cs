using Godot;

namespace PengfuNailongMod.PengfuNailongModCode.Visuals;

public partial class NailongRiggedMotion : Node
{
    private const string RigScenePath = MainFile.ResPath + "/scenes/combat/nailong_rigged_visual.tscn";
    private const string SourceTextureName = "character_model_nailong.png";
    private static readonly List<WeakReference<NailongRiggedMotion>> ActiveRigs = [];

    private readonly Dictionary<Node2D, BonePose> basePoses = [];
    private Node2D? rootBone;
    private Node2D? bodyBone;
    private Node2D? bellyBone;
    private Node2D? headBone;
    private Node2D? leftArmBone;
    private Node2D? rightArmBone;
    private Node2D? leftHandBone;
    private Node2D? rightHandBone;
    private Node2D? leftLegBone;
    private Node2D? rightLegBone;
    private double elapsed;
    private float attackPulse;
    private float blockPulse;
    private float drawPulse;
    private float energyPulse;
    private float powerPulse;
    private float emotionPulse;

    private readonly record struct BonePose(Vector2 Position, Vector2 Scale, float RotationDegrees);

    public static void Attach(Node root)
    {
        if (root.GetNodeOrNull<NailongRiggedMotion>("NailongRiggedMotion") != null) return;

        root.AddChild(new NailongRiggedMotion
        {
            Name = "NailongRiggedMotion"
        });
    }

    public static void React(NailongActionKind kind)
    {
        ActiveRigs.RemoveAll(reference =>
        {
            if (!reference.TryGetTarget(out NailongRiggedMotion? rig) || !GodotObject.IsInstanceValid(rig))
            {
                return true;
            }

            rig.ReactInstance(kind);
            return false;
        });
    }

    public override void _Ready()
    {
        CallDeferred(nameof(InstallRig));
    }

    public void InstallRig()
    {
        Sprite2D? source = FindSourceSprite(GetParent());
        Node? parent = source?.GetParent();
        PackedScene? scene = ResourceLoader.Load<PackedScene>(RigScenePath);
        Node2D? rig = scene?.Instantiate<Node2D>();
        if (source == null || parent == null || rig == null)
        {
            GD.PushWarning("PengfuNailong: failed to install rigged combat visual.");
            return;
        }

        int sourceIndex = source.GetIndex();
        parent.AddChild(rig);
        rig.Name = "NailongRigInstance";
        rig.Position = source.Position;
        rig.Rotation = source.Rotation;
        rig.Scale = source.Scale;
        rig.Modulate = source.Modulate;
        rig.ZIndex = source.ZIndex;
        parent.MoveChild(rig, sourceIndex);
        HideSourceSprites(GetParent());

        BindBones(rig);
        if (rootBone == null)
        {
            GD.PushWarning("PengfuNailong: rigged combat visual is missing RootBone.");
            return;
        }

        ActiveRigs.Add(new WeakReference<NailongRiggedMotion>(this));
    }

    public override void _Process(double delta)
    {
        if (rootBone == null) return;

        elapsed += delta;
        attackPulse = Decay(attackPulse, delta, 2.7f);
        blockPulse = Decay(blockPulse, delta, 3.0f);
        drawPulse = Decay(drawPulse, delta, 3.4f);
        energyPulse = Decay(energyPulse, delta, 3.1f);
        powerPulse = Decay(powerPulse, delta, 2.9f);
        emotionPulse = Decay(emotionPulse, delta, 2.8f);

        float wave = Mathf.Sin((float)elapsed * 2.1f);
        float slow = Mathf.Sin((float)elapsed * 1.35f);
        float breath = wave * 0.014f;
        float bob = Mathf.Sin((float)elapsed * 2.1f + Mathf.Pi * 0.5f) * 3.2f;

        Apply(rootBone, new Vector2(attackPulse * 72.0f - blockPulse * 9.0f, bob - attackPulse * 8.0f + blockPulse * 3.0f - energyPulse * 10.0f), Vector2.One, 0.0f);
        Apply(bodyBone, Vector2.Zero, new Vector2(1.0f + breath + blockPulse * 0.04f + powerPulse * 0.035f, 1.0f - breath * 0.55f + attackPulse * 0.035f + energyPulse * 0.025f), slow * 0.5f - attackPulse * 1.6f + powerPulse * 1.8f);
        Apply(bellyBone, new Vector2(attackPulse * 5.0f, emotionPulse * 2.5f - energyPulse * 4.0f), new Vector2(1.0f + 0.02f + emotionPulse * 0.075f + blockPulse * 0.025f + energyPulse * 0.09f + powerPulse * 0.04f, 1.0f - 0.012f + blockPulse * 0.055f + energyPulse * 0.06f), 0.0f);
        Apply(headBone, new Vector2(attackPulse * 10.0f + drawPulse * 4.0f, -emotionPulse * 2.5f - drawPulse * 6.0f - energyPulse * 5.0f), Vector2.One, slow * 2.2f - attackPulse * 8.0f + blockPulse * 2.0f + drawPulse * 4.5f);
        Apply(leftArmBone, new Vector2(blockPulse * 2.0f - drawPulse * 7.0f, -drawPulse * 9.0f), Vector2.One, wave * 1.1f + blockPulse * 4.0f + emotionPulse * 3.0f + drawPulse * 18.0f - powerPulse * 5.0f);
        Apply(rightArmBone, new Vector2(attackPulse * 24.0f + drawPulse * 7.0f, -attackPulse * 7.0f - drawPulse * 9.0f), Vector2.One, -wave * 1.3f - attackPulse * 42.0f - blockPulse * 3.5f - drawPulse * 18.0f + powerPulse * 5.0f);
        Apply(leftHandBone, new Vector2(blockPulse * 1.5f - drawPulse * 6.0f, -drawPulse * 10.0f), Vector2.One, wave * 2.0f + blockPulse * 5.0f + drawPulse * 16.0f);
        Apply(rightHandBone, new Vector2(attackPulse * 35.0f + drawPulse * 6.0f, -attackPulse * 10.0f - drawPulse * 10.0f), Vector2.One, -wave * 2.0f - attackPulse * 55.0f - drawPulse * 16.0f);
        Apply(leftLegBone, new Vector2(0.0f, -bob * 0.22f), Vector2.One, -wave * 0.8f);
        Apply(rightLegBone, new Vector2(0.0f, bob * 0.18f), Vector2.One, wave * 0.8f);
    }

    private void ReactInstance(NailongActionKind kind)
    {
        switch (kind)
        {
            case NailongActionKind.Attack:
            case NailongActionKind.NailongStrike:
            case NailongActionKind.MultiHitAttack:
            case NailongActionKind.HeavyAttack:
                attackPulse = kind == NailongActionKind.HeavyAttack ? 1.35f : 1.0f;
                break;
            case NailongActionKind.NailongDefend:
            case NailongActionKind.Block:
            case NailongActionKind.BonusBlock:
                blockPulse = 1.0f;
                break;
            case NailongActionKind.Draw:
                drawPulse = 1.0f;
                break;
            case NailongActionKind.Energy:
                energyPulse = 1.0f;
                break;
            case NailongActionKind.Power:
                powerPulse = 1.0f;
                break;
            default:
                emotionPulse = 1.0f;
                break;
        }
    }

    private void BindBones(Node root)
    {
        rootBone = root.GetNodeOrNull<Node2D>("RootBone");
        bodyBone = root.GetNodeOrNull<Node2D>("RootBone/BodyBone");
        bellyBone = root.GetNodeOrNull<Node2D>("RootBone/BellyBone");
        headBone = root.GetNodeOrNull<Node2D>("RootBone/HeadBone");
        leftArmBone = root.GetNodeOrNull<Node2D>("RootBone/LeftArmBone");
        rightArmBone = root.GetNodeOrNull<Node2D>("RootBone/RightArmBone");
        leftHandBone = root.GetNodeOrNull<Node2D>("RootBone/LeftHandBone");
        rightHandBone = root.GetNodeOrNull<Node2D>("RootBone/RightHandBone");
        leftLegBone = root.GetNodeOrNull<Node2D>("RootBone/LeftLegBone");
        rightLegBone = root.GetNodeOrNull<Node2D>("RootBone/RightLegBone");

        foreach (Node2D? bone in new[] { rootBone, bodyBone, bellyBone, headBone, leftArmBone, rightArmBone, leftHandBone, rightHandBone, leftLegBone, rightLegBone })
        {
            if (bone != null) basePoses[bone] = new BonePose(bone.Position, bone.Scale, bone.RotationDegrees);
        }
    }

    private void Apply(Node2D? bone, Vector2 positionOffset, Vector2 scale, float rotationOffset)
    {
        if (bone == null || !basePoses.TryGetValue(bone, out BonePose pose)) return;

        bone.Position = pose.Position + positionOffset;
        bone.Scale = pose.Scale * scale;
        bone.RotationDegrees = pose.RotationDegrees + rotationOffset;
    }

    private static float Decay(float value, double delta, float speed)
    {
        return Mathf.Max(0.0f, value - (float)delta * speed);
    }

    private static Sprite2D? FindSourceSprite(Node? root)
    {
        if (root == null) return null;

        foreach (Node child in root.GetChildren())
        {
            if (child is Sprite2D sprite && sprite.Visible && IsSourceSprite(sprite))
            {
                return sprite;
            }

            Sprite2D? nested = FindSourceSprite(child);
            if (nested != null) return nested;
        }

        return null;
    }

    private static void HideSourceSprites(Node? root)
    {
        if (root == null) return;

        foreach (Node child in root.GetChildren())
        {
            if (child is Sprite2D sprite && IsSourceSprite(sprite))
            {
                sprite.Visible = false;
            }

            HideSourceSprites(child);
        }
    }

    private static bool IsSourceSprite(Sprite2D sprite)
    {
        return sprite.Texture?.ResourcePath.EndsWith(SourceTextureName, StringComparison.Ordinal) == true;
    }
}
