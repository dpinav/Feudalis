using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace Feudalis.Utils
{
    public class FeudalisChatLib
    {

        public static void DebugConsolePrint(string message)
        {
            if (!GameNetwork.IsMultiplayer)
                FeudalisChatLib.ChatMessage(message, Colors.Yellow);
            Console.WriteLine(message);
            Debug.Print(message);
        }

        public static void ChatMessage(string message) => InformationManager.DisplayMessage(new InformationMessage(message));

        public static void ChatMessage(string message, Color color) => InformationManager.DisplayMessage(new InformationMessage(message, color));

        public static void BannerMessage(string message, string soundEventPath = "") => MBInformationManager.AddQuickInformation(new TextObject(message), soundEventPath: soundEventPath);

        public static void BroadcastChatMessage(string message, Color color, bool writeToConsole = false)
        {
            if (GameNetwork.IsServer)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage)new ServerBroadcastInformationMessage(message, color));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                if (writeToConsole)
                    FeudalisChatLib.DebugConsolePrint(message);
            }
            if (GameNetwork.IsMultiplayer)
                return;
            FeudalisChatLib.ChatMessage(message, color);
        }
    }
}
