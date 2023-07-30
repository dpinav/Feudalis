using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class MissionMultiplayerFeudalis : MissionMultiplayerSiege
    {
        private const int InitialGold = 1500;
        private float _lastPerkTickTime;

        private MissionScoreboardComponent _missionScoreboardComponent;

        public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
        {
            return MissionLobbyComponent.MultiplayerGameType.Siege;
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
            if ((double)Mission.Current.CurrentTime - (double)this._lastPerkTickTime < 1.0)
                return;
            this._lastPerkTickTime = Mission.Current.CurrentTime;
            MPPerkObject.TickAllPeerPerks((int)((double)this._lastPerkTickTime / 1.0));

            //GameNetwork.BeginBroadcastModuleEvent();
            //GameNetwork.WriteMessage((GameNetworkMessage)new SiegeMoraleChangeMessage(1000, 1000, this._capturePointRemainingMoraleGains));
            //GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }

        public override bool CheckForMatchEnd() => false;
        public override bool CheckForRoundEnd() => false;
        public override bool CheckIfOvertime() => false;
        public override bool CheckForWarmupEnd() => true;
    }
}
