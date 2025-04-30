using Content.Shared.Humanoid;
using Content.Shared.StatusIcon;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.Damage;

namespace Content.Shared.Mindflayer;

[RegisterComponent, NetworkedComponent]
public sealed partial class MindflayerComponent : Component
{

    public readonly List<ProtoId<EntityPrototype>> BaseMindflayerActions = new()
    {
        "ActionSwarmMenu",
        "ActionToggleSwarmProd",
        "ActionActivateQuickReboot",
        "ActionDrainMind"
    };

    [DataField("soundMeatPool")]
    public List<SoundSpecifier?> SoundPool = new()
    {
        new SoundPathSpecifier("/Audio/_EinsteinEngines/Effects/Buzzes/buzz1.ogg"),
        new SoundPathSpecifier("/Audio/_EinsteinEngines/Effects/Buzzes/buzz2.ogg"),
        new SoundPathSpecifier("/Audio/_EinsteinEngines/Effects/Buzzes/buzz3.ogg"),
    };

    public Dictionary<string, EntityUid?> Equipment = new();

    public bool EMPImmune = false;

    [DataField("PassiveHealing")]
    public DamageSpecifier PassiveHealing = new()
    {
        DamageDict = new()
        {
            { "Blunt", -2 },
            { "Slash", -2 },
            { "Piercing", -2 },
            {"Ion", -2}
        }
    };
}

[DataDefinition]
public sealed partial class TransformData
{
    /// <summary>
    ///     Entity's name.
    /// </summary>
    [DataField]
    public string Name;

    /// <summary>
    ///     Entity's fingerprint, if it exists.
    /// </summary>
    [DataField]
    public string? Fingerprint;

    /// <summary>
    ///     Entity's humanoid appearance component.
    /// </summary>
    [ViewVariables(VVAccess.ReadOnly), NonSerialized]
    public HumanoidAppearanceComponent Appearance;
}
