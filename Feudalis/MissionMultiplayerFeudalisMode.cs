using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;

namespace Feudalis
{
    public class MissionMultiplayerFeudalisMode : MissionBasedMultiplayerGameMode
    {
        public MissionMultiplayerFeudalisMode(string name) : base(name) { }

        public override void StartMultiplayerGame(string scene)
        {
            MissionState.OpenNew(
                "Feudalis",
                new MissionInitializerRecord(scene),
                missionController =>
                {
                    if (GameNetwork.IsServer)
                    {
                        return new MissionBehavior[]
                        {
                        MissionLobbyComponent.CreateBehavior(),
                        new MissionMultiplayerFeudalis(),
                        new MissionMultiplayerFeudalisClient(),
                        new MultiplayerTimerComponent(),
                        new MultiplayerMissionAgentVisualSpawnComponent(),
                        new ConsoleMatchStartEndHandler(),
                        new SpawnComponent(new FeudalisSpawnFrameBehavior(), new FeudalisSpawningBehavior()),
                        new MissionLobbyEquipmentNetworkComponent(),
                        new MultiplayerTeamSelectComponent(),
                        new MissionHardBorderPlacer(),
                        new MissionBoundaryPlacer(),
                        new MissionBoundaryCrossingHandler(),
                        new MultiplayerPollComponent(),
                        new MultiplayerAdminComponent(),
                        new MultiplayerGameNotificationsComponent(),
                        new MissionOptionsComponent(),
                        new MissionScoreboardComponent(new FeudalisScoreboardData()),
                        new MissionAgentPanicHandler(),
                        new AgentHumanAILogic(),
                        new EquipmentControllerLeaveLogic(),
                        new MultiplayerPreloadHelper(),
                        };
                    }
                    else
                    {
                        return new MissionBehavior[]
                        {
                        MissionLobbyComponent.CreateBehavior(),
                        new MissionMultiplayerFeudalisClient(),
                        new MultiplayerAchievementComponent(),
                        new MultiplayerTimerComponent(),
                        new MultiplayerMissionAgentVisualSpawnComponent(),
                        new ConsoleMatchStartEndHandler(),
                        new MissionLobbyEquipmentNetworkComponent(),
                        new MultiplayerTeamSelectComponent(),
                        new MissionHardBorderPlacer(),
                        new MissionBoundaryPlacer(),
                        new MissionBoundaryCrossingHandler(),
                        new MultiplayerPollComponent(),
                        new MultiplayerGameNotificationsComponent(),
                        new MissionOptionsComponent(),
                        new MissionScoreboardComponent(new FeudalisScoreboardData()),
                        new MissionMatchHistoryComponent(),
                        new EquipmentControllerLeaveLogic(),
                        new MissionRecentPlayersComponent(),
                        new MultiplayerPreloadHelper(),
                        };
                    }
                }
            );
        }
    }
}
