using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{

    /* A wrapper of DestructableComponent class */
    public class PBRepairableComponent : ScriptComponentBehavior
    {
        private float _hitpoint;
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

            if (DestructableComponent is null)
            {
                Debug.Print($"DestructableComponent not found for PBRepairableComponent with GameEntity Name: {GameEntity.Name}");
                return;
            }

            this.DestructableComponent.OnHitTaken += OnHit;

            _hitpoint = DestructableComponent.HitPoint;

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

        private void OnHit(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
        {
            if (!IsDestroyed)
            {
                _hitpoint = DestructableComponent.HitPoint;
                return;
            }

            if (!UsesResources())
            {
                _hitpoint += 2 * inflictedDamage;
            }

            if (_hitpoint >= DestructableComponent.MaxHitPoint)
            {
                DestructableComponent.Reset();
                _hitpoint = DestructableComponent.HitPoint;
            }

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
            return _resourcesNeeded.Count > 0 || ResourcesTypeNeeded is not null && ResourcesAmountNeeded is not null && !ResourcesTypeNeeded.IsEmpty() && !ResourcesAmountNeeded.IsEmpty();
        }

    }
}

