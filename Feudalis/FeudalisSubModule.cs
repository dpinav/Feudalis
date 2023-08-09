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

            AddPostfix(harmony, typeof(MultiplayerWarmupComponent), "CanMatchStartAfterWarmup",
                BindingFlags.Public | BindingFlags.Instance, typeof(PatchNoMatchEnd), nameof(PatchNoMatchEnd.Postfix));

            /*
            AddPostfix(harmony, typeof(MissionRepresentativeBase), "Gold.get",
                BindingFlags.Public | BindingFlags.Instance, typeof(PatchRepresentativeGold), nameof(PatchRepresentativeGold.Postfix));
            */
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.GameTextManager.LoadGameTexts();
        }

        private static void AddPrefix(Harmony harmony, Type classToPatch, string functionToPatchName, BindingFlags flags, Type patchClass, string functionPatchName)
        {
            var functionToPatch = classToPatch.GetMethod(functionToPatchName, flags);
            var newHarmonyPatch = patchClass.GetMethod(functionPatchName);
            harmony.Patch(functionToPatch, prefix: new HarmonyMethod(newHarmonyPatch));
        }

        private static void AddPostfix(Harmony harmony, Type classToPatch, string functionToPatchName, BindingFlags flags, Type patchClass, string functionPatchName)
        {
            var functionToPatch = classToPatch.GetMethod(functionToPatchName, flags);
            var newHarmonyPatch = patchClass.GetMethod(functionPatchName);
            harmony.Patch(functionToPatch, postfix: new HarmonyMethod(newHarmonyPatch));
        }
    }
}
