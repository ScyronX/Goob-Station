using Content.Server.Administration.Systems;
using Content.Server.DoAfter;
using Content.Server.Forensics;
using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Server.Store.Systems;
using Content.Server.Zombies;
using Content.Shared.Alert;
using Content.Shared.Mindflayer;
using Content.Shared.Chemistry.Components;
using Content.Shared.Cuffs.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Nutrition.Components;
using Content.Shared.Store.Components;
using Robust.Server.Audio;
using Robust.Shared.Audio;
using Robust.Shared.Random;
using Content.Shared.Popups;
using Content.Shared.Damage;
using Robust.Shared.Prototypes;
using Content.Server.Body.Systems;
using Content.Shared.Actions;
using Content.Shared.Polymorph;
using Robust.Shared.Serialization.Manager;
using Content.Server.Actions;
using Content.Server.Humanoid;
using Content.Server.Polymorph.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Server.Flash;
using Content.Server.Emp;
using Robust.Server.GameObjects;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Content.Shared.Damage.Systems;
using Content.Shared.Mind;
using Content.Server.Objectives.Components;
using Content.Server.Light.EntitySystems;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Cuffs;
using Content.Shared.Fluids;
using Content.Shared.Revolutionary.Components;
using Robust.Shared.Player;
using System.Numerics;
using Content.Shared.Camera;
using Robust.Shared.Timing;
using Content.Shared.Damage.Components;
using Content.Server.Gravity;
using Content.Shared.Mobs.Components;
using Content.Server.Stunnable;
using Content.Shared.Jittering;
using Content.Server.Explosion.EntitySystems;
using System.Linq;
using Content.Shared.Heretic;
using Content.Shared._Goobstation.Actions;
using Content.Shared.Body.Components;
using Content.Shared.PowerCell.Components;
using Content.Server.PowerCell;

namespace Content.Server.Mindflayer;

public sealed partial class MindflayerSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _rand = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly BodySystem _bodySystem = default!;
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly RejuvenateSystem _rejuv =default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly DoAfterSystem _doAfter = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly StoreSystem _store = default!;

    public EntProtoId SwarmProdPrototype = "Swarmprod";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MindflayerComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<MindflayerComponent, EmpAttemptEvent>(OnEmpAttempt);

        SubscribeAbilities();
    }

    public void PlayRoboticSound(EntityUid uid, MindflayerComponent comp)
    {
        var rand = _rand.Next(0, comp.SoundPool.Count - 1);
        var sound = comp.SoundPool.ToArray()[rand];
        _popup.PopupEntity(Loc.GetString("silicon-behavior-buzz"), uid);
        Spawn("EffectSparks", Transform(uid).Coordinates);
        _audio.PlayPvs(sound, uid, AudioParams.Default.WithVolume(-3f));
    }

    public bool TryUseAbility(EntityUid uid, MindflayerComponent comp, BaseActionEvent action)
    {
        if (action.Handled)
            return false;

        action.Handled = true;

        return true;
    }

    public bool TryToggleItem(EntityUid uid, EntProtoId proto, MindflayerComponent comp)
    {
        if (!comp.Equipment.TryGetValue(proto.Id, out var item))
        {
            item = Spawn(proto, Transform(uid).Coordinates);
            if (!_hands.TryForcePickupAnyHand(uid, (EntityUid) item))
            {
                _popup.PopupEntity(Loc.GetString("mindflayer-fail-hands"), uid, uid);
                QueueDel(item);
                return false;
            }
            comp.Equipment.Add(proto.Id, item);
            return true;
        }

        QueueDel(item);
        comp.Equipment.Remove(proto.Id);

        return true;
    }

    public bool IsIncapacitated(EntityUid uid)
    {
        if (_mobState.IsIncapacitated(uid)
        || (TryComp<CuffableComponent>(uid, out var cuffs) && cuffs.CuffedHandCount > 0))
            return true;

        return false;
    }

    #region Event Handlers

    private void OnStartup(EntityUid uid, MindflayerComponent comp, ref ComponentStartup args)
    {

        foreach (var actionId in comp.BaseMindflayerActions)
            _actions.AddAction(uid, actionId);


        foreach (var (id, part) in _bodySystem.GetBodyChildren(uid))
        {
            part.CanSever = false;
            Dirty(id, part);
        }

    }

    private void OnEmpAttempt(EntityUid uid, MindflayerComponent comp, EmpAttemptEvent args)
    {
        // IPC is immune to emp
        // powercell relays the event to body
        args.Cancel();
    }

    #endregion
}
