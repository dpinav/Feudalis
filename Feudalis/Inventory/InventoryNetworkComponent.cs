using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis.Inventory
{
    public class InventoryNetworkComponent : MissionNetwork
    {
        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            if (!GameNetwork.IsServer)
            {
                return;
            }

            registerer.Register<ClientRequestMoveItem>(new GameNetworkMessage.ClientMessageHandlerDelegate<ClientRequestMoveItem>(HandleClientRequestMoveItem));
        }

        private bool HandleClientRequestMoveItem(NetworkCommunicator peer, ClientRequestMoveItem message)
        {
            InventoryMissionRepresentative representative = peer.GetComponent<InventoryMissionRepresentative>();
            Inventory inventory = representative.Inventory;
            Agent agent = peer.ControlledAgent;

            if (representative is null || inventory is null || agent is null)
            {
                return false;
            }

            if (message.SourcePanelIndex == 0 && message.TargetPanelIndex == 0)
            {
                TryMoveFromCentralToCentral(message.SourceSlotIndex, message.TargetSlotIndex, inventory);
            }
            if (message.SourcePanelIndex == 0 && message.TargetPanelIndex == 1)
            {
                TryEquipArmorFromCentral();
            }
        }

        private void TryMoveFromCentralToCentral(int sourceSlotIndex, int targetSlotIndex, Inventory inventory)
        {

            InventorySlot sourceSlot = inventory.Storage[sourceSlotIndex];
            InventorySlot targetSlot = inventory.Storage[targetSlotIndex];

            if (sourceSlot is null || sourceSlot.IsEmpty())
            {
                return;
            }

            if (targetSlot is null || !targetSlot.IsEmpty())
            {
                return;
            }

            targetSlot.Item = sourceSlot.Item;
            sourceSlot.Item = null;
        }
    }
}
