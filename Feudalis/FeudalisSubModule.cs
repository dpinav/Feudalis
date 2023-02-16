using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ModuleManager;
using System.IO;
using System.Xml.Linq;

namespace Feudalis
{
    public class FeudalisSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            Module.CurrentModule.AddMultiplayerGameMode(new MissionMultiplayerFeudalisMode("Feudalis"));
            Console.WriteLine("Feudalis mode added");
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.GameTextManager.LoadGameTexts();
        }

    }
}
