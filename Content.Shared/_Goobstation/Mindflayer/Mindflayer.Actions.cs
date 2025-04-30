using Content.Shared.Actions;
using Robust.Shared.GameStates;

namespace Content.Shared.Mindflayer;

[RegisterComponent, NetworkedComponent]
public sealed partial class MindflayerActionComponent : Component
{

    //[DataField] public bool RequirePower = false;

}

public sealed partial class OpenSwarmMenuEvent : InstantActionEvent { }
public sealed partial class ToggleSwarmProdEvent : InstantActionEvent { }
public sealed partial class ActivateQuickRebootEvent : InstantActionEvent { }
public sealed partial class DrainMindEvent : EntityTargetActionEvent { }

[DataDefinition]
public sealed partial class MindflayerArmorPurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerFluidFeetPurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerFaradayCagePurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerInsulationPurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerNaniteHealingPurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerEnhancedOpticalSensitivityPurchasedEvent : EntityEventArgs;
public sealed partial class MindflayerReinforcedJointsPurchasedEvent : EntityEventArgs;
