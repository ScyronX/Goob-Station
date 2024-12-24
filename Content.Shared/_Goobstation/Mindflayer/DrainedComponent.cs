using Robust.Shared.GameStates;

namespace Content.Shared.Mindflayer;


/// <summary>
///     Component that indicates that a person's mind has been drained by a Mindflayer.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(MindDrainedSystem))]
public sealed partial class MindDrainedComponent : Component
{

}
