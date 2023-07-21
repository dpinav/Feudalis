using Feudalis.Utils;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis.Inventory
{
    public class InventoryMissionRepresentative : MissionRepresentativeBase
    {
        public Inventory Inventory { get; private set; }

        public InventoryMissionRepresentative()
        {
            Inventory = new Inventory();
        }

        public static InventoryMissionRepresentative GetInventoryRepresentative(
            NetworkCommunicator playerNetwork = null)
        {
            if (GameNetwork.IsClient)
                return GameNetwork.MyPeer.GetComponent<InventoryMissionRepresentative>();
            if (GameNetwork.IsServer)
                return playerNetwork.GetComponent<InventoryMissionRepresentative>();
            return (InventoryMissionRepresentative)null;
        }

        public override void OnFinalize()
        {
            Inventory = null;
        }

        public void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            if (!GameNetwork.IsClient)
                return;

            GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            handlerRegisterer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerUpdateInventoryMessage>(HandleServerUpdateInventoryMessage));
        }

        private void HandleServerUpdateInventoryMessage(ServerUpdateInventoryMessage message)
        {
            Inventory = message.Inventory;
        }

        public bool MoveItemToStorage(int targetSlotIndex, int targetPanelIndex, int sourceSlotIndex, int sourcePanelIndex)
        {
            if (GameNetwork.IsClient)
            {
                // Inform server
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new ClientRequestMoveItem(targetSlotIndex, targetPanelIndex, sourceSlotIndex, sourcePanelIndex));
                GameNetwork.EndModuleEventAsClient();
                FeudalisChatLib.ChatMessage("CLIENT: Moved Item from " + sourceSlotIndex + " to " + targetSlotIndex, Colors.Magenta);
                return true;
            }

            if (GameNetwork.IsServer)
            {
                // Inform server
                GameNetwork.BeginModuleEventAsClient();
                GameNetwork.WriteMessage(new ServerUpdateInventoryMessage(Inventory));
                GameNetwork.EndModuleEventAsClient();
                FeudalisChatLib.ChatMessage("SERVER: Moved Item from " + sourceSlotIndex + " to " + targetSlotIndex, Colors.Magenta);
                return true;
            }

            return false;
        }
    }
}