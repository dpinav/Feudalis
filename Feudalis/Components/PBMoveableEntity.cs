using Feudalis.Utils;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{
    // An Entity that may be moved to different states, such as a gate or drawbridge
    // A PBUseMoveableEntityComponent is required on one of the parent's entities
    // The Entity with PBUseMoveableEntityComponent will specifically be the one that is interacted with, but not necessarily the one that moves 
    public class PBMoveableEntity : ScriptComponentBehavior
    {
        public float MovementSpeed = 1f;
        public bool IsLooping = false;
        public bool StopOnEachPoint = false;
        public bool StopOnUse = false;

        // Entity with this tag will be the one that is moved
        public string TagBaseEntity = "pbmoveablebase";

        // Entities with these tags will mark the path that the moveable entity will move along
        // They will be deleted on initialization, only their position, rotation and scale is saved
        public string TagsChildrenPath = "point1";

        private bool _isMoving = false;
        private GameEntity _baseEntity;
        private SynchedMissionObject _baseSynchedMissionObject;
        private List<MatrixFrame> _pointsToMoveTo = new List<MatrixFrame>();
        private int _currentPointIndex = 1;
        private MatrixFrame _currentFrame;
        private float _distanceTraveled = 0f; // Distance traveled between points, [0, 100], decimals included

        public bool IsMovingBackwards { get; set; }
        public bool IsMoving => this._isMoving;
        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;


        protected override void OnInit()
        {
            base.OnInit();

            this._baseEntity = GameEntity.GetFirstChildEntityWithTag(TagBaseEntity);

            if (this._baseEntity is null)
            {
                return;
            }

            if (!this.GameEntity.HasTag("pbmoveableentity"))
            {
                this.GameEntity.AddTag("pbmoveableentity");
            }

            if (!this._baseEntity.HasScriptOfType<SynchedMissionObject>())
            {
                this._baseEntity.CreateAndAddScriptComponent("SynchedMissionObject");
            }

            this._currentFrame = this._baseEntity.GetFrame();
            this._baseSynchedMissionObject = this._baseEntity.GetFirstScriptOfType<SynchedMissionObject>();

            // Initial point
            this._pointsToMoveTo.Add(this._baseEntity.GetFrame());

            string[] tagsChildren = this.TagsChildrenPath.Split(',');

            foreach (string tag in tagsChildren)
            {
                GameEntity child = GameEntity.GetFirstChildEntityWithTag(tag);

                if (child is null)
                {
                    continue;
                }

                MatrixFrame frame = child.GetFrame();
                this._pointsToMoveTo.Add(frame);
                child.Remove(0);
            }

            this.IsMovingBackwards = false;
            this.SetScriptComponentToTick(GetTickRequirement());
        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!this._isMoving || GameNetwork.IsClientOrReplay)
            {
                return;
            }

            if (this.IsAtEnd())
            {
                if (!this.IsLooping)
                {
                    this._isMoving = false;
                    return;
                }
                this.IsMovingBackwards = !this.IsMovingBackwards;
                this._currentPointIndex += !this.IsMovingBackwards ? 2 : -2;
            }

            this._distanceTraveled += this.MovementSpeed * dt;
            this._distanceTraveled = MathF.Clamp(this._distanceTraveled, 0f, 1f);

            MatrixFrame currentFrame = this._currentFrame;
            MatrixFrame destinationFrame = this._pointsToMoveTo[this._currentPointIndex];
            MatrixFrame nextFrame = MatrixFrame.Slerp(currentFrame, destinationFrame, this._distanceTraveled);
            nextFrame.Scale(currentFrame.GetScale());

            this._baseSynchedMissionObject.SetFrameSynched(ref nextFrame);

            if (this._distanceTraveled >= 1f)
            {
                FeudalisChatLib.BroadcastChatMessage("**Reached distance " + this._distanceTraveled +
                    "\n **currentPoint: " + this._currentPointIndex +
                    "\n **IsAtEnd: " + this.IsAtEnd() +
                    "\n **IsMovingBackwards: " + this.IsMovingBackwards +
                    "\n **StopOnUse: " + this.StopOnUse
                    , Colors.Magenta, true);

                if (this.StopOnEachPoint)
                {
                    this._isMoving = false;
                }
                this._currentFrame = this._pointsToMoveTo[this._currentPointIndex];
                this._distanceTraveled = 0f;
                this._currentPointIndex += !this.IsMovingBackwards ? 1 : -1;
            }

        }

        public void OnUse()
        {

            if (GameNetwork.IsClientOrReplay)
            {
                return;
            }
            FeudalisChatLib.BroadcastChatMessage("distance " + this._distanceTraveled +
                    "\n currentPoint: " + this._currentPointIndex +
                    "\n isMoving: " + this._isMoving +
                    "\n IsAtEnd: " + this.IsAtEnd() +
                    "\n IsMovingBackwards: " + this.IsMovingBackwards +
                    "\n StopOnUse: " + this.StopOnUse
                    , Colors.Blue, true);

            if (this._isMoving && this.StopOnUse)
            {
                this.OnStop();
                return;
            }

            if (!this._isMoving)
            {
                this.OnMove();
            }
        }

        public void OnMove()
        {
            if (this._isMoving)
            {
                return;
            }

            this._isMoving = true;
        }

        public void OnStop()
        {
            if (!this._isMoving)
            {
                return;
            }

            this._isMoving = false;
        }

        public bool IsAtEnd()
        {
            return !this.IsMovingBackwards ? this._currentPointIndex >= this._pointsToMoveTo.Count : this._currentPointIndex < 0;
        }


        protected override bool OnCheckForProblems()
        {
            bool result = base.OnCheckForProblems();

            return result;
        }

    }
}
