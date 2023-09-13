using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;


namespace Feudalis.Components
{

    /* A wrapper of DestructableComponent class */
    public class PBRepairableComponent : SynchedMissionObject
    {
        private float _hitpoint;
        private bool _isActuallyDestroyed = false; // Prevents the destroying hit to repair at the same time. Won't be needed when we use resources
        private Dictionary<string, int> _resourcesNeeded = new Dictionary<string, int>();

        public string ResourcesTypeNeeded;
        public string ResourcesAmountNeeded;

        public DestructableComponent DestructableComponent { get; private set; }


        public bool IsDestroyed => DestructableComponent.IsDestroyed;

        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;

        protected override void OnInit()
        {
            base.OnInit();
            DestructableComponent = GameEntity.GetFirstScriptOfType<DestructableComponent>();

            if (DestructableComponent == null)
            {
                Console.WriteLine($"DestructableComponent not found for PBRepairableComponent with GameEntity Name: {GameEntity.Name}");
                return;
            }

            // this.DestructableComponent.OnHitTaken += OnHit;

            _hitpoint = DestructableComponent.HitPoint;
            Console.WriteLine($"PBRepairableComponent initialized for: {GameEntity.Name}");
            Console.WriteLine($"Max hitpoints: {_hitpoint}");

            SetScriptComponentToTick(GetTickRequirement());

            if (!UsesResources())
            {
                return;
            }

            // Parse the strings into a dictionary
            string[] types = ResourcesTypeNeeded.Split(',');
            string[] amounts = ResourcesAmountNeeded.Split(',');
            if (types.Length == amounts.Length)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    _resourcesNeeded.Add(types[i], int.Parse(amounts[i]));
                }
            }

        }

        protected override bool OnHit(Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, out bool reportDamage)
        {
            reportDamage = false;
            if (GameNetwork.IsClientOrReplay)
            {
                return false;
            }

            Console.WriteLine($"OnHit PBRepairableComponent: {GameEntity.Name}");

            if (!IsDestroyed)
            {
                _hitpoint -= damage;
                Console.WriteLine($"OnHit damaging, hitpoints: {_hitpoint}");
                return false;
            }

            if (!UsesResources() && _isActuallyDestroyed)
            {
                _hitpoint += damage;
                Console.WriteLine($"OnHit repairing, hitpoints: {_hitpoint}");
            }
            else
            {
                // Shenagigans that will be removed once we use resources for repairing
                _hitpoint = 0;
                _isActuallyDestroyed = true;
            }

            if (_hitpoint >= DestructableComponent.MaxHitPoint)
            {
                Console.WriteLine($"OnHit PBRepairableComponent REPAIRED");
                DestructableComponent.Reset();
                _hitpoint = DestructableComponent.HitPoint;
                _isActuallyDestroyed = false;

                // Reset only works for server side, so we use a little network message hack
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new NetworkMessages.FromServer.SyncObjectDestructionLevel(DestructableComponent, 1, -1, 0f, impactPosition, impactDirection));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
            }

            reportDamage = true;
            return true;
        }

        protected override bool OnCheckForProblems()
        {
            bool result = base.OnCheckForProblems();

            if (GameEntity.GetFirstScriptOfType<DestructableComponent>() is null)
            {
                MBEditor.AddEntityWarning(GameEntity, "PBRepairableComponent: DestructableComponent not found");
                result = true;
            }

            return result;
        }

        private bool UsesResources()
        {
            return _resourcesNeeded.Count > 0 ||
                ResourcesTypeNeeded != null && ResourcesAmountNeeded != null &&
                !ResourcesTypeNeeded.IsEmpty() && !ResourcesAmountNeeded.IsEmpty();
        }

    }
}
