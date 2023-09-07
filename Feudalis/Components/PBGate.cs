using Feudalis.Utils;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{
    public class PBGate : PBUsableMissionObject
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

        public bool IsDestroyed => PBRepairableComponent is not null && PBRepairableComponent.IsDestroyed;

        public bool IsOpen => State == GateState.Open || IsDestroyed;

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
            SetOpenAndClosedMatrixFrames();
            SetScriptComponentToTick(GetTickRequirement());
        }

        protected override void OnEditorInit()
        {
            base.OnEditorInit();
            this.LockUserFrames = false;
            this.LockUserPositions = false;
            this.IsInstantUse = true;
            this.ActionMessage = new TextObject("Door");
            this.DescriptionMessage = new TextObject("Press F");
            SetOpenAndClosedMatrixFrames();
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
            this._currentRotation += RotationSpeed * dt;

            // Door is being opened, goes from closed position to open position
            if (this.IsOpen)
            {
                SlerpGameEntity(_closedPosition, _openPosition, _currentRotation);
            }
            // Door is being closed, goes from open position to closed position
            else
            {
                SlerpGameEntity(_openPosition, _closedPosition, _currentRotation);
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
                MatrixFrame entityFrame = GameEntity.GetFrame();
                this._openPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(TotalRotation);
                this._closedPosition = entityFrame;
            }
            else
            {
                MatrixFrame entityFrame = GameEntity.GetFrame();
                this._closedPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(-TotalRotation);
                this._openPosition = entityFrame;
            }
        }

        private void SlerpGameEntity(MatrixFrame origin, MatrixFrame destination, float rotationPercentage)
        {
            MatrixFrame matrixFrame = MatrixFrame.Slerp(origin, destination, rotationPercentage);
            SetFrameSynched(ref matrixFrame);
        }

        private void OpenGate()
        {
            this.State = GateState.Open;
            this._isRotating = true;
            this._currentRotation = 0;
        }

        private void CloseGate()
        {
            this.State = GateState.Closed;
            this._isRotating = true;
            this._currentRotation = 0;

        }

        protected override void OnEditorVariableChanged(string variableName)
        {
            switch (variableName)
            {
                case "State":
                    if (this.IsOpen)
                    {
                        GameEntity.SetFrame(ref _openPosition);
                    }
                    else
                    {
                        GameEntity.SetFrame(ref _closedPosition);
                    }
                    break;
                case "RotationSpeed":
                    this.RotationSpeed = MathF.Clamp(RotationSpeed, 0.1f, 100f);
                    break;
                case "TotalRotation":
                    SetOpenAndClosedMatrixFrames();
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
