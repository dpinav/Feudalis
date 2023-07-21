using Feudalis.ClientOnly.Inventory;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace Feudalis
{
    [ViewCreatorModule]
    public class FeudalisMissionView
    {
        [ViewMethod("Feudalis")]
        public static MissionView[] OpenFeudalisMission(Mission mission)
        {
            List<MissionView> missionViews = new List<MissionView>();

            missionViews.Add(ViewCreator.CreateMissionServerStatusUIHandler());
            missionViews.Add(ViewCreator.CreateMissionMultiplayerPreloadView(mission));
            missionViews.Add(ViewCreator.CreateMultiplayerTeamSelectUIHandler());
            missionViews.Add(ViewCreator.CreateMissionKillNotificationUIHandler());
            missionViews.Add(ViewCreator.CreateMissionAgentStatusUIHandler(mission));
            missionViews.Add(ViewCreator.CreateMissionMainAgentEquipmentController(mission));
            missionViews.Add(ViewCreator.CreateMissionMainAgentCheerBarkControllerView(mission));
            missionViews.Add(ViewCreator.CreateMissionMultiplayerEscapeMenu("Feudalis"));
            missionViews.Add(ViewCreator.CreateMissionScoreBoardUIHandler(mission, false));
            missionViews.Add(ViewCreator.CreateMultiplayerEndOfRoundUIHandler());
            missionViews.Add(ViewCreator.CreateMultiplayerEndOfBattleUIHandler());
            missionViews.Add(ViewCreator.CreateLobbyEquipmentUIHandler());
            missionViews.Add(ViewCreator.CreateMissionAgentLabelUIHandler(mission));
            missionViews.Add(ViewCreator.CreatePollProgressUIHandler());
            missionViews.Add(ViewCreator.CreateMissionFlagMarkerUIHandler());
            missionViews.Add(ViewCreator.CreateMultiplayerMissionDeathCardUIHandler());
            missionViews.Add(ViewCreator.CreateOptionsUIHandler());
            missionViews.Add(ViewCreator.CreateMissionMainAgentEquipDropView(mission));
            missionViews.Add(ViewCreator.CreateMissionBoundaryCrossingView());
            missionViews.Add(new MissionBoundaryWallView());
            missionViews.Add(new MissionItemContourControllerView());
            missionViews.Add(new MissionAgentContourControllerView());
            missionViews.Add(new InventoryMissionView());
            return missionViews.ToArray();
        }
    }
}
  