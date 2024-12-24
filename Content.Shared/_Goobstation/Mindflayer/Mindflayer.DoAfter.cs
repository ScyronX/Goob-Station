using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared.Mindflayer;

[Serializable, NetSerializable]
public sealed partial class DrainMindDoAfterEvent : SimpleDoAfterEvent { }