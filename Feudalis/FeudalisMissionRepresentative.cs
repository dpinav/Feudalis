using System;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class FeudalisMissionRepresentative : MissionRepresentativeBase
    {
        public int Bounty { get; private set; }

        public Action<NetworkCommunicator, int> OnPeerBountyUpdated;

        public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            if (GameNetwork.IsClient)
            {
                GameNetwork.NetworkMessageHandlerRegisterer reg = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
                reg.Register<FeudalisPointsUpdateMessage>(HandleServerEventBountyPointsUpdate);
            }
        }

        private void HandleServerEventBountyPointsUpdate(FeudalisPointsUpdateMessage message)
        {
            var representative = message.NetworkCommunicator.GetComponent<MissionRepresentativeBase>();

            if (representative is FeudalisMissionRepresentative bountyRepresentative)
            {
                bountyRepresentative.UpdateBounty(message.Bounty);
                OnPeerBountyUpdated?.Invoke(message.NetworkCommunicator, message.Bounty);
            }
        }

        public void UpdateBounty(int bounty)
        {
            Bounty = bounty;
        }
    }
}
