using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis.Inventory
{

    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ServerUpdateInventoryMessage : GameNetworkMessage
    {
        public Inventory Inventory { get; private set; }

        public ServerUpdateInventoryMessage() { }

        public ServerUpdateInventoryMessage(Inventory inventory)
        {
            Inventory = inventory;
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            // Read the items from the packet
            // CentralPanelItems = ... (Implement reading a list of ItemObject from the packet)
            // LeftPanelArmorItems = ... (Implement reading a list of ItemObject from the packet)
            // RightPanelWeaponItems = ... (Implement reading a list of ItemObject from the packet)

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            // Write the items to the packet
            // ... (Implement writing a list of ItemObject to the packet for CentralPanelItems, LeftPanelArmorItems, and RightPanelWeaponItems)
        }

        protected override MultiplayerMessageFilter OnGetLogFilter()
        {
            throw new System.NotImplementedException();
        }

        protected override string OnGetLogFormat()
        {
            throw new System.NotImplementedException();
        }
    }
}