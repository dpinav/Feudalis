using System;
using TaleWorlds.MountAndBlade;

namespace Feudalis
{
    public class MissionMultiplayerFeudalisClient : MissionMultiplayerTeamDeathmatchClient
    {
        public override bool IsGameModeUsingGold => true;
        public override bool IsGameModeTactical => false;
        public override bool IsGameModeUsingRoundCountdown => false;
        public override bool IsGameModeUsingAllowCultureChange => false;
        public override bool IsGameModeUsingAllowTroopChange => false;
        public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;

        public FeudalisMissionRepresentative FeudalisRepresentative { get; private set; }
        public Action OnMyRepresentativeAssigned;

        private void OnMyClientSynchronized()
        {
            FeudalisRepresentative = GameNetwork.MyPeer.GetComponent<FeudalisMissionRepresentative>();
            FeudalisRepresentative?.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);

            OnMyRepresentativeAssigned?.Invoke();
        }

        public override void OnBehaviorInitialize()
        {
            base.OnBehaviorInitialize();

            MissionNetworkComponent.OnMyClientSynchronized += OnMyClientSynchronized;
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();

            MissionNetworkComponent.OnMyClientSynchronized -= OnMyClientSynchronized;
            FeudalisRepresentative?.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            base.AddRemoveMessageHandlers(registerer);

            if (GameNetwork.IsClient)
            {
                registerer.Register<NetworkMessages.FromServer.SyncGoldsForSkirmish>(HandleServerEventUpdateGold);
            }
        }

        private void HandleServerEventUpdateGold(NetworkMessages.FromServer.SyncGoldsForSkirmish message)
        {
            var representative = message.VirtualPlayer.GetComponent<MissionRepresentativeBase>();
            OnGoldAmountChangedForRepresentative(representative, message.GoldAmount);
        }

        public override void OnGoldAmountChangedForRepresentative(MissionRepresentativeBase representative, int goldAmount)
        {
            base.OnGoldAmountChangedForRepresentative(representative, goldAmount);

            if (representative != null && MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending)
            {
                representative.UpdateGold(2000);
            }
        }

        public override int GetGoldAmount()
        {
            return 2000;
        }
    }
}
