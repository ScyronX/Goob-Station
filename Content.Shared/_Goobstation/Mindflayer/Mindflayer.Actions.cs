using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.Mindflayer;

[RegisterComponent, NetworkedComponent]
public sealed partial class MindflayerActionComponent : Component
{

    //[DataField] public bool RequirePower = false;

}

public sealed partial class ToggleSwarmProdEvent : InstantActionEvent { }
public sealed partial class ActivateQuickRebootEvent : InstantActionEvent { }
public sealed partial class DrainMindEvent : EntityTargetActionEvent { }