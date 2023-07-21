using Feudalis.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace Feudalis
{
    public class MissionMultiplayerFeudalis : MissionMultiplayerGameModeBase
    {
        private const int InitialGold = 1500;
        private const int InitialBounty = 3;

        public override bool IsGameModeHidingAllAgentVisuals => true;
        public override bool IsGameModeUsingOpposingTeams => true;

        private MissionScoreboardComponent _missionScoreboardComponent;

        public override MissionLobbyComponent.MultiplayerGameType GetMissionType()
        {
            return MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();
            _missionScoreboardComponent = Mission.GetMissionBehavior<MissionScoreboardComponent>();
        }

        public override void AfterStart()
        {
            string attackerCultureStringId = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
            string defenderCultureStringId = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();

            var attackerCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(attackerCultureStringId);
            var defenderCulture = MBObjectManager.Instance.GetObject<BasicCultureObject>(defenderCultureStringId);

            var bannerTeamOne = new Banner(attackerCulture.BannerKey, attackerCulture.BackgroundColor1, attackerCulture.ForegroundColor1);
            var bannerTeamTwo = new Banner(defenderCulture.BannerKey, defenderCulture.BackgroundColor2, defenderCulture.ForegroundColor2);

            Mission.Teams.Add(BattleSideEnum.Attacker, attackerCulture.BackgroundColor1, attackerCulture.ForegroundColor1, bannerTeamOne);
            Mission.Teams.Add(BattleSideEnum.Defender, defenderCulture.BackgroundColor2, defenderCulture.ForegroundColor2, bannerTeamTwo);
        }

        protected override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
        {
            networkPeer.AddComponent<FeudalisMissionRepresentative>();
            networkPeer.AddComponent<InventoryMissionRepresentative>();
        }

        protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
        {
            ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), InitialGold);
            GameModeBaseClient.OnGoldAmountChangedForRepresentative(networkPeer.GetComponent<FeudalisMissionRepresentative>(), InitialGold);
            ChangeBountyForPeer(networkPeer.GetComponent<FeudalisMissionRepresentative>(), InitialBounty);

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

        public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
        {
            if (oldTeam != null && oldTeam != newTeam && oldTeam.Side != BattleSideEnum.None)
            {
                ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), InitialGold);
            }
        }

        public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
        {
            if (blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed))
            {
                if (affectedAgent?.IsHuman == true)
                {
                    if (affectorAgent != null && affectorAgent.IsEnemyOf(affectedAgent))
                    {
                        var affectedAgentRepresentative = affectedAgent.MissionPeer.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>();
                        var affectorAgentRepresentative = affectorAgent.MissionPeer.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>();

                        ChangeBountyForPeer(affectorAgentRepresentative, affectorAgentRepresentative.Bounty + affectedAgentRepresentative.Bounty / 2);
                    }

                    var affectedAgentMissionPeer = affectedAgent.MissionPeer;
                    if (affectedAgentMissionPeer != null)
                    {
                        ChangeBountyForPeer(affectedAgentMissionPeer.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>(), InitialBounty);
                        ChangeCurrentGoldForPeer(affectedAgentMissionPeer, InitialGold);
                    }
                }
            }
        }

        public void ChangeBountyForPeer(FeudalisMissionRepresentative representative, int newAmount)
        {
            Debug.Assert(GameNetwork.IsServerOrRecorder, "GameNetwork.IsServerOrRecorder");

            if (representative.Peer.Communicator.IsConnectionActive)
            {
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage(new FeudalisPointsUpdateMessage(representative, newAmount));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                representative.UpdateBounty(newAmount);
            }
        }

        public override Team GetWinnerTeam()
        {
            var scoreSides = _missionScoreboardComponent.Sides;
            var attackerBounty = 0;
            var defenderBounty = 0;

            foreach (var player in scoreSides[(int)BattleSideEnum.Attacker].Players)
            {
                attackerBounty += player.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>().Bounty;
            }

            foreach (var player in scoreSides[(int)BattleSideEnum.Defender].Players)
            {
                defenderBounty += player.GetNetworkPeer().GetComponent<FeudalisMissionRepresentative>().Bounty;
            }

            if (attackerBounty > defenderBounty)
            {
                return Mission.Teams.Attacker;
            }
            else if (defenderBounty > attackerBounty)
            {
                return Mission.Teams.Defender;
            }
            else
            {
                return null;
            }
        }
    }
}
