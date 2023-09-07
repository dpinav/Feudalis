using Feudalis.Utils;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{
    public class PBUseMoveableEntity : PBUsableMissionObject
    {
        public float RotationSpeedOnUse = 0f;
        public bool LockUserPosition = false;
        public bool InstantUse = true;
        public bool UseBackwards = false;

        private List<PBMoveableEntity> _moveableEntities;

        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;

        public override string GetDescriptionText(GameEntity gameEntity = null)
        {
            return "Use";
        }

        protected override void OnInit()
        {
            base.OnInit();
            LockUserFrames = false;
            LockUserPositions = this.LockUserPosition;
            IsInstantUse = this.InstantUse;
            ActionMessage = new TextObject("Use");
            DescriptionMessage = new TextObject("Press F");

            List<GameEntity> entitiesWithScript = this.GameEntity.Parent.CollectChildrenEntitiesWithTag("pbmoveableentity");
            this._moveableEntities = entitiesWithScript.Select(e => e.GetScriptComponents<PBMoveableEntity>().FirstOrDefault()).ToList();

            this.SetScriptComponentToTick(GetTickRequirement());

            foreach (PBMoveableEntity entity in this._moveableEntities)
            {
                FeudalisChatLib.ChatMessage("Found moveable entity " + entity.TagBaseEntity);
            }
        }

        public override void OnUse(Agent userAgent)
        {
            base.OnUse(userAgent);

            foreach (PBMoveableEntity entity in this._moveableEntities)
            {
                if (this.UseBackwards)
                {
                    entity.IsMovingBackwards = true;
                }
                entity.OnUse();
                FeudalisChatLib.ChatMessage("Used moveable entity " + entity.TagBaseEntity);
            }
        }
    }
}
