using Content.Shared.Examine;
using Content.Shared.Mobs;

namespace Content.Shared.Mindflayer;

public sealed partial class MindDrainedSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MindDrainedComponent, ExaminedEvent>(OnExamine);
        SubscribeLocalEvent<MindDrainedComponent, MobStateChangedEvent>(OnMobStateChange);
    }

    private void OnExamine(Entity<MindDrainedComponent> ent, ref ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("mindflayer-drain-onexamine"));
    }

    private void OnMobStateChange(Entity<MindDrainedComponent> ent, ref MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            RemComp<MindDrainedComponent>(ent);
    }
}
