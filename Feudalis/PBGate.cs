using Feudalis.Utils;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class PBGate : UsableMissionObject
    {

        private float _currentRotation = 0f;
        private bool _isRotating = false;

        private MatrixFrame _openPosition;
        private MatrixFrame _closedPosition;

        public GateState State = GateState.Closed;

        public DoorOwnership OwningTeam;

        // Total degrees that the gate will rotate from open to close or backwards
        public float TotalRotation = 30f;

        // Rotation speed, from 0.1 to 100
        public float RotationSpeed = 3f;

        public PBRepairableComponent PBRepairableComponent { get; private set; }

        public bool IsDestroyed => this.PBRepairableComponent is not null && this.PBRepairableComponent.IsDestroyed;

        public bool IsOpen => this.State == PBGate.GateState.Open || this.IsDestroyed;

        public override FocusableObjectType FocusableObjectType => FocusableObjectType.Item;

        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;

        public override string GetDescriptionText(GameEntity gameEntity = null)
        {
            return "Press F";
        }

        protected override void OnInit()
        {
            base.OnInit();
            this.LockUserFrames = false;
            this.LockUserPositions = false;
            this.IsInstantUse = true;
            this.ActionMessage = new TextObject("Door");
            this.DescriptionMessage = new TextObject("Press F");
            this.SetOpenAndClosedMatrixFrames();
            this.SetScriptComponentToTick(this.GetTickRequirement());
        }

        protected override void OnEditorInit()
        {
            base.OnEditorInit();
            this.LockUserFrames = false;
            this.LockUserPositions = false;
            this.IsInstantUse = true;
            this.ActionMessage = new TextObject("Door");
            this.DescriptionMessage = new TextObject("Press F");
            this.SetOpenAndClosedMatrixFrames();
        }

        public override void OnUse(Agent userAgent)
        {
            base.OnUse(userAgent);

            if (this._isRotating)
            {
                return;
            }

            FeudalisChatLib.ChatMessage($"{userAgent?.Name} used the door", Colors.Magenta);

            if (this.IsOpen)
            {
                CloseGate();
            }
            else
            {
                OpenGate();
            }

        }

        protected override void OnTick(float dt)
        {
            base.OnTick(dt);
            if (!this._isRotating || GameNetwork.IsClient)
            {
                return;
            }

            // Slerp Alpha, from 0 to 1
            this._currentRotation += (this.RotationSpeed) * dt;

            // Door is being opened, goes from closed position to open position
            if (this.IsOpen)
            {
                SlerpGameEntity(this._closedPosition, this._openPosition, this._currentRotation);
            }
            // Door is being closed, goes from open position to closed position
            else
            {
                SlerpGameEntity(this._openPosition, this._closedPosition, this._currentRotation);
            }

            if (this._currentRotation >= 1)
            {
                this._isRotating = false;
                this._currentRotation = 0;
            }
        }

        private void SetOpenAndClosedMatrixFrames()
        {
            if (this.IsOpen)
            {
                MatrixFrame entityFrame = this.GameEntity.GetFrame();
                this._openPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(this.TotalRotation);
                this._closedPosition = entityFrame;
            }
            else
            {
                MatrixFrame entityFrame = this.GameEntity.GetFrame();
                this._closedPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(-this.TotalRotation);
                this._openPosition = entityFrame;
            }
        }

        private void SlerpGameEntity(MatrixFrame origin, MatrixFrame destination, float rotationPercentage)
        {
            MatrixFrame matrixFrame = MatrixFrame.Slerp(origin, destination, rotationPercentage);
            this.SetFrameSynched(ref matrixFrame);
        }

        private void OpenGate()
        {
            this.State = GateState.Open;
            this._isRotating = true;
            this._currentRotation = 0;

            // this.SetFrameSynchedOverTime(ref this._openPosition, 150f);
        }

        private void CloseGate()
        {
            this.State = GateState.Closed;
            this._isRotating = true;
            this._currentRotation = 0;

            // this.SetFrameSynchedOverTime(ref this._closedPosition, 150f);
        }

        protected override void OnEditorVariableChanged(string variableName)
        {
            switch (variableName)
            {
                case "State":
                    if (this.IsOpen)
                    {
                        this.GameEntity.SetFrame(ref this._openPosition);
                    }
                    else
                    {
                        this.GameEntity.SetFrame(ref this._closedPosition);
                    }
                    break;
                case "RotationSpeed":
                    this.RotationSpeed = MathF.Clamp(this.RotationSpeed, 0.1f, 100f);
                    break;
                case "TotalRotation":
                    this.SetOpenAndClosedMatrixFrames();
                    break;
            }
        }

        public enum DoorOwnership
        {
            Defenders,
            Attackers,
        }

        public enum GateState
        {
            Open,
            Closed,
        }
    }
}
