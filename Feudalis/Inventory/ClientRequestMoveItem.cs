using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;


namespace Feudalis.Inventory
{

    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class ClientRequestMoveItem : GameNetworkMessage
    {

        public static CompressionInfo.Integer invasionHealthCompression = new CompressionInfo.Integer(0, 30, true);

        public int TargetSlotIndex { get; private set; }
        public int TargetPanelIndex { get; private set; }

        public int SourceSlotIndex { get; private set; }
        public int SourcePanelIndex { get; private set; }


        public ClientRequestMoveItem() { }

        public ClientRequestMoveItem(int targetSlotIndex, int targetPanelIndex, int sourceSlotIndex, int sourcePanelIndex)
        {
            TargetSlotIndex = targetSlotIndex;
            TargetPanelIndex = targetPanelIndex;

            SourceSlotIndex = sourcePanelIndex;
            SourceSlotIndex = sourceSlotIndex;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Equipment;

        protected override string OnGetLogFormat() => $"Requesting move item: Move {SourceSlotIndex} to {TargetSlotIndex}";


        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            TargetSlotIndex = ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
            TargetPanelIndex = ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
            SourceSlotIndex = ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
            SourcePanelIndex = ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteIntToPacket(TargetPanelIndex, CompressionMission.ItemSlotCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(TargetSlotIndex, CompressionMission.ItemSlotCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(SourceSlotIndex, CompressionMission.ItemSlotCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(SourcePanelIndex, CompressionMission.ItemSlotCompressionInfo);
        }
    }
}