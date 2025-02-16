using Content.Shared.Store;
using Robust.Shared.Prototypes;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(MindflayerRuleSystem))]
public sealed partial class MindflayerRuleComponent : Component
{
    public readonly List<EntityUid> MindflayerMinds = new();

    public readonly List<ProtoId<StoreCategoryPrototype>> StoreCategories = new()
    {
        "MindflayerPassiveAbilities",
        "MindflayerIntruderAbilities",
        "MindflayerDestroyerAbilities"
    };
}
