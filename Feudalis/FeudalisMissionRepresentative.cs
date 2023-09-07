using Feudalis.Utils;
using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis
{
    public class FeudalisMissionRepresentative : MissionRepresentativeBase
    {
        public int Bounty { get; private set; }

        public Action<NetworkCommunicator, int> OnPeerBountyUpdated;

        public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            if (!GameNetwork.IsClient)
            {
                return;
            }
            GameNetwork.NetworkMessageHandlerRegisterer reg = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            reg.Register<FeudalisPointsUpdateMessage>(HandleServerEventBountyPointsUpdate);
            reg.Register<ServerBroadcastInformationMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerBroadcastInformationMessage>(this.DisplayBroadcastMessage));
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

        private void DisplayBroadcastMessage(ServerBroadcastInformationMessage message)
        {
            if (message.IsBannerMessage)
                FeudalisChatLib.BannerMessage(message.Message);
            else
                FeudalisChatLib.ChatMessage(message.Message, message.Color);
        }
    }
}
