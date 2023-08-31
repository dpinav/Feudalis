using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{

    /* A wrapper of DestructableComponent class */
    public class PBRepairableComponent : SynchedMissionObject
    {
        private float _hitpoint;
        private Dictionary<string, int> _resourcesNeeded = new Dictionary<string, int>();

        public string ResourcesTypeNeeded;
        public string ResourcesAmountNeeded;

        public DestructableComponent DestructableComponent { get; private set; }


        public bool IsDestroyed => this.DestructableComponent.IsDestroyed;

        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;

        protected override void OnInit()
        {
            base.OnInit();
            DestructableComponent = this.GameEntity.GetFirstScriptOfType<DestructableComponent>();

            if (DestructableComponent is null)
            {
                Debug.Print($"DestructableComponent not found for PBRepairableComponent with GameEntity Name: {this.GameEntity.Name}");
                return;
            }

            this._hitpoint = this.DestructableComponent.HitPoint;

            this.SetScriptComponentToTick(this.GetTickRequirement());

            if (!this.UsesResources())
            {
                return;
            }

            // Parse the strings into a dictionary
            string[] types = this.ResourcesTypeNeeded.Split(',');
            string[] amounts = this.ResourcesAmountNeeded.Split(',');
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
            reportDamage = true;

            if (!this.IsDestroyed)
            {
                this._hitpoint = this.DestructableComponent.HitPoint;
                return true;
            }

            if (!this.UsesResources())
            {
                this._hitpoint += 2 * damage;
            }

            if (this._hitpoint >= this.DestructableComponent.MaxHitPoint)
            {
                this.DestructableComponent.Reset();
                this._hitpoint = this.DestructableComponent.HitPoint;
            }

            return true;
        }

        protected override bool OnCheckForProblems()
        {
            bool result = base.OnCheckForProblems();

            if (this.GameEntity.GetFirstScriptOfType<DestructableComponent>() is null)
            {
                MBEditor.AddEntityWarning(this.GameEntity, "PBRepairableComponent: DestructableComponent not found");
                result = true;
            }

            return result;
        }

        private bool UsesResources()
        {
            return this._resourcesNeeded.Count > 0 || this.ResourcesTypeNeeded is not null && this.ResourcesAmountNeeded is not null && !this.ResourcesTypeNeeded.IsEmpty() && !this.ResourcesAmountNeeded.IsEmpty();
        }

    }
}

