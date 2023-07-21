using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis.Inventory
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class ClientUpdateInventoryMessage : GameNetworkMessage
    {
        public ClientUpdateInventoryMessage() { }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Equipment;

        protected override string OnGetLogFormat() => "Requesting inventory update";


        protected override bool OnRead() => true;

        protected override void OnWrite() { }
    }


}
