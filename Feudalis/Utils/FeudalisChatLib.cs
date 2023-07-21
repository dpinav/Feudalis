using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Feudalis.Utils
{
    public class FeudalisChatLib
    {

        public static void ChatMessage(string message) => InformationManager.DisplayMessage(new InformationMessage(message));

        public static void ChatMessage(string message, Color color) => InformationManager.DisplayMessage(new InformationMessage(message, color));

        public static void BannerMessage(string message, string soundEventPath = "") => MBInformationManager.AddQuickInformation(new TextObject(message), soundEventPath: soundEventPath);

    }
}
