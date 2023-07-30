using HarmonyLib;
using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class FeudalisSubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new MissionMultiplayerFeudalisMode("Feudalis"));
            Console.WriteLine("Feudalis mode added");

            Harmony.DEBUG = true;

            var harmony = new Harmony("feudalis");
            // harmony.PatchAll(assembly);
            var original = typeof(MultiplayerWarmupComponent).GetMethod("CanMatchStartAfterWarmup", BindingFlags.Public | BindingFlags.Instance);
            var postfix = typeof(PatchNoMatchEnd).GetMethod("Postfix");
            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.GameTextManager.LoadGameTexts();
        }

    }
}
