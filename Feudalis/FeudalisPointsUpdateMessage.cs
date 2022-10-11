using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class FeudalisPointsUpdateMessage : GameNetworkMessage
    {
        public int Bounty { get; private set; }
        public NetworkCommunicator NetworkCommunicator { get; private set; }

        public FeudalisPointsUpdateMessage() { }

        public FeudalisPointsUpdateMessage(FeudalisMissionRepresentative representative, int bounty)
        {
            Bounty = bounty;
            NetworkCommunicator = representative.GetNetworkPeer();
        }

        protected override void OnWrite()
        {
            WriteIntToPacket(Bounty, CompressionMatchmaker.ScoreCompressionInfo);
            WriteNetworkPeerReferenceToPacket(NetworkCommunicator);
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            Bounty = ReadIntFromPacket(CompressionMatchmaker.ScoreCompressionInfo, ref bufferReadValid);
            NetworkCommunicator = ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            return MultiplayerMessageFilter.GameMode;
        }

        protected override string OnGetLogFormat()
        {
            return "Feudalis Point Update";
        }
    }
}
