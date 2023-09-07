using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ServerBroadcastInformationMessage : GameNetworkMessage
    {
        private static CompressionInfo.Float colorVectorCompression = new CompressionInfo.Float(0.0f, (float)byte.MaxValue, 8);

        public string Message { get; private set; }

        public Color Color { get; private set; }

        public bool IsBannerMessage { get; private set; }

        public ServerBroadcastInformationMessage()
        {
        }

        public ServerBroadcastInformationMessage(string message, Color color, bool isBannerMessage = false)
        {
            this.Message = message;
            this.Color = color;
            this.IsBannerMessage = isBannerMessage;
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Messaging;

        protected override string OnGetLogFormat() => "Broadcast information message: " + this.Message;

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            this.Message = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            this.Color = Color.ConvertStringToColor(GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid));
            this.IsBannerMessage = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
            Color color = this.Color;
            if (false)
                this.Color = Colors.White;
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(this.Message);
            GameNetworkMessage.WriteStringToPacket(this.Color.ToString());
            GameNetworkMessage.WriteBoolToPacket(this.IsBannerMessage);
        }
    }
}
