using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace Feudalis.Components
{
    public class PBDoor : UsableMissionObject
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

        public DestructableComponent DestructableComponent { get; private set; }

        public bool IsDestroyed => DestructableComponent != null && DestructableComponent.IsDestroyed;

        public bool IsOpen => State == GateState.Open;

        public override FocusableObjectType FocusableObjectType => FocusableObjectType.Item;

        public override TickRequirement GetTickRequirement() => TickRequirement.Tick;

        public override string GetDescriptionText(GameEntity gameEntity = null)
        {
            return "Press F";
        }

        protected override void OnInit()
        {
            base.OnInit();
            LockUserFrames = false;
            LockUserPositions = false;
            IsInstantUse = true;
            TextObject textObject = new TextObject("Press {KEY} to use", null);
            textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13)));
            ActionMessage = textObject;
            DescriptionMessage = new TextObject("Door");

            DestructableComponent = GameEntity.GetFirstScriptOfType<DestructableComponent>();

            SetOpenAndClosedMatrixFrames();
            SetScriptComponentToTick(GetTickRequirement());
        }

        protected override void OnEditorInit()
        {
            base.OnEditorInit();
            LockUserFrames = false;
            LockUserPositions = false;
            IsInstantUse = true;
            ActionMessage = new TextObject("Door");
            DescriptionMessage = new TextObject("Press F");
            SetOpenAndClosedMatrixFrames();
        }

        public override void OnUse(Agent userAgent)
        {
            base.OnUse(userAgent);

            if (_isRotating || IsDestroyed)
            {
                return;
            }

            if (IsOpen)
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
            if (!_isRotating || GameNetwork.IsClient)
            {
                return;
            }

            // Slerp Alpha, from 0 to 1
            _currentRotation += RotationSpeed * dt;

            // Door is being opened, goes from closed position to open position
            if (IsOpen)
            {
                SlerpGameEntity(_closedPosition, _openPosition, _currentRotation);
            }
            // Door is being closed, goes from open position to closed position
            else
            {
                SlerpGameEntity(_openPosition, _closedPosition, _currentRotation);
            }

            if (_currentRotation >= 1)
            {
                _isRotating = false;
                _currentRotation = 0;
            }
        }

        private void SetOpenAndClosedMatrixFrames()
        {
            if (IsOpen)
            {
                MatrixFrame entityFrame = GameEntity.GetFrame();
                _openPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(TotalRotation);
                _closedPosition = entityFrame;
            }
            else
            {
                MatrixFrame entityFrame = GameEntity.GetFrame();
                _closedPosition = entityFrame;
                entityFrame.rotation.RotateAboutUp(-TotalRotation);
                _openPosition = entityFrame;
            }
        }

        private void SlerpGameEntity(MatrixFrame origin, MatrixFrame destination, float rotationPercentage)
        {
            MatrixFrame matrixFrame = MatrixFrame.Slerp(origin, destination, rotationPercentage);
            SetFrameSynched(ref matrixFrame);
        }

        private void OpenGate()
        {
            if (IsOpen) return;
            State = GateState.Open;
            _isRotating = true;
            _currentRotation = 0;
        }

        private void CloseGate()
        {
            if (!IsOpen) return;
            State = GateState.Closed;
            _isRotating = true;
            _currentRotation = 0;

        }

        protected override void OnEditorVariableChanged(string variableName)
        {
            switch (variableName)
            {
                case "State":
                    if (IsOpen)
                    {
                        GameEntity.SetFrame(ref _openPosition);
                    }
                    else
                    {
                        GameEntity.SetFrame(ref _closedPosition);
                    }
                    break;
                case "RotationSpeed":
                    RotationSpeed = MathF.Clamp(RotationSpeed, 0.1f, 100f);
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
