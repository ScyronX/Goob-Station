using Content.Shared.Mindflayer;
using Content.Shared.Chemistry.Components;
using Content.Shared.Cuffs.Components;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.Store.Components;
using Content.Shared.Popups;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Shared.Damage.Prototypes;
using Content.Server.Objectives.Components;
using Content.Server.Light.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Server.Flash.Components;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Stealth.Components;
using Content.Shared.Damage.Components;
using Content.Server.Radio.Components;

namespace Content.Server.Mindflayer;

public sealed partial class MindflayerSystem : EntitySystem
{
    public void SubscribeAbilities()
    {
        SubscribeLocalEvent<MindflayerComponent, OpenSwarmMenuEvent>(OnOpenSwarmMenu);
        SubscribeLocalEvent<MindflayerComponent, ToggleSwarmProdEvent>(OnToggleSwarmProd);
        SubscribeLocalEvent<MindflayerComponent, ActivateQuickRebootEvent>(OnActivateQuickReboot);
        SubscribeLocalEvent<MindflayerComponent, DrainMindEvent>(OnDrainMind);
        SubscribeLocalEvent<MindflayerComponent, DrainMindDoAfterEvent>(OnDrainMindDoAfter);

    }

    private void OnOpenSwarmMenu(EntityUid uid, MindflayerComponent comp, ref OpenSwarmMenuEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;

        _store.ToggleUi(uid, uid, store);
    }

    private void OnDrainMind(EntityUid uid, MindflayerComponent comp, ref DrainMindEvent args)
    {
        var target = args.Target;

        if (!IsIncapacitated(target))
        {
            _popup.PopupEntity(Loc.GetString("mindflayer-drain-fail-incapacitated"), uid, uid);
            return;
        }
        if (HasComp<MindDrainedComponent>(target))
        {
            _popup.PopupEntity(Loc.GetString("mindflayer-drain-fail-drained"), uid, uid);
            return;
        }

        if (!TryUseAbility(uid, comp, args))
            return;

        var popupOthers = Loc.GetString("mindflayer-drain-start", ("user", Identity.Entity(uid, EntityManager)), ("target", Identity.Entity(target, EntityManager)));
        _popup.PopupEntity(popupOthers, uid, PopupType.LargeCaution);
        PlayRoboticSound(uid, comp);
        var dargs = new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(15), new DrainMindDoAfterEvent(), uid, target)
        {
            DistanceThreshold = 1.5f,
            BreakOnDamage = true,
            BreakOnHandChange = false,
            BreakOnMove = true,
            BreakOnWeightlessMove = true,
            AttemptFrequency = AttemptFrequency.StartAndEnd
        };
        _doAfter.TryStartDoAfter(dargs);
    }
    public ProtoId<DamageGroupPrototype> DrainedDamageGroup = "Genetic";
    private void OnDrainMindDoAfter(EntityUid uid, MindflayerComponent comp, ref DrainMindDoAfterEvent args)
    {
        if (args.Args.Target == null)
            return;

        var target = args.Args.Target.Value;

        if (args.Cancelled || !IsIncapacitated(target) || HasComp<MindDrainedComponent>(target))
            return;

        PlayRoboticSound(args.User, comp);

        var dmg = new DamageSpecifier(_proto.Index(DrainedDamageGroup), 200);
        _damage.TryChangeDamage(target, dmg, false, false);

        EnsureComp<MindDrainedComponent>(target);

        var popup = Loc.GetString("mindflayer-drain-end");

        if (TryComp<StoreComponent>(args.User, out var store))
        {
            _store.TryAddCurrency(new Dictionary<string, FixedPoint2> { { "Swarms", 1f} }, args.User, store);
            _store.UpdateUserInterface(args.User, args.User, store);
        }

       /* if (_mind.TryGetMind(uid, out var mindId, out var mind))
            if (_mind.TryGetObjectiveComp<AbsorbConditionComponent>(mindId, out var objective, mind))
                objective.Absorbed += 1;*/
        var popupOthersend = Loc.GetString("mindflayer-drain-end", ("user", Identity.Entity(uid, EntityManager)), ("target", Identity.Entity(target, EntityManager)));
    }

    public void OnActivateQuickReboot(EntityUid uid, MindflayerComponent comp, ref ActivateQuickRebootEvent args)
    {
        if (!TryUseAbility(uid, comp, args))
            return;

        _rejuv.PerformRejuvenate(uid);

        PlayRoboticSound(uid, comp);
    }

    private void OnToggleSwarmProd(EntityUid uid, MindflayerComponent comp, ref ToggleSwarmProdEvent args)
    {
        if (!TryUseAbility(uid, comp, args))
            return;

        if (!TryToggleItem(uid, SwarmProdPrototype, comp))
            return;

        PlayRoboticSound(uid, comp);
    }

}
