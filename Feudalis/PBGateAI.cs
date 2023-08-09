using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class PBGateAI : UsableMachineAIBase
    {
        private PBGate.GateState _initialState;

        public void ResetInitialGateState(PBGate.GateState newInitialState) => this._initialState = newInitialState;

        public PBGateAI(PBGate gate)
          : base((UsableMachine)gate)
        {
            this._initialState = gate.State;
        }

        public override bool HasActionCompleted => ((PBGate)this.UsableMachine).State != this._initialState;
    }
}
