using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class MissionMultiplayerFeudalis : MissionMultiplayerTeamDeathmatch
    {
        private const int InitialGold = 1500;
        private float _lastPerkTickTime;

        private MissionScoreboardComponent _missionScoreboardComponent;

        public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
        {
            return MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            _missionScoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();
            _lastPerkTickTime = Mission.Current.CurrentTime;
        }

        protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
        {
            base.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
            networkPeer.AddComponent<FeudalisMissionRepresentative>();
        }

        protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
        {
            base.HandleEarlyNewClientAfterLoadingFinished(networkPeer);

            ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), InitialGold);
            GameModeBaseClient.OnGoldAmountChangedForRepresentative(networkPeer.GetComponent<FeudalisMissionRepresentative>(), InitialGold);
            foreach (var side in _missionScoreboardComponent.Sides)
            {
                if (side != null)
                {
                    foreach (var player in side.Players)
                    {
                        var playerRepresentative = player.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>();
                        GameNetwork.BeginModuleEventAsServer(networkPeer);
                        GameNetwork.WriteMessage(new FeudalisPointsUpdateMessage(playerRepresentative, playerRepresentative.Bounty));
                        GameNetwork.EndModuleEventAsServer();
                    }
                }
            }
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
        }

        public override bool CheckForMatchEnd() => false;
        public override bool CheckForRoundEnd() => false;
        public override bool CheckIfOvertime() => false;
        public override bool CheckForWarmupEnd() => true;
    }
}
