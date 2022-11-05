using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryVM : ViewModel
    {
        private MissionMultiplayerFeudalisClient _client;
        private readonly MissionMultiplayerGameModeBaseClient _gameMode;
        private bool _isMyRepresentativeAssigned;
        private MissionScoreboardComponent _scoreboardComponent;
        private bool _isEnabled;
        private string _remainingRoundTime;
        private MBBindingList<FeudalisPlayerVM> _players;

        private MBList<InventorySlotVM> _inventorySlots;
        private InventoryArmorPanelVM _armorVM;
        private InventoryWeaponPanelVM _weaponVM;

        private const float UpdatePlayersDuration = 5.0f;
        private float _updatePlayersTimeElapsed = 0.0f;

        public InventoryVM(MissionMultiplayerFeudalisClient client, int inventoryCapacity)
        {
            _client = client;
            _client.OnMyRepresentativeAssigned += OnMyRepresentativeAssigned;
            _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();

            _scoreboardComponent = Mission.Current.GetMissionBehavior<MissionScoreboardComponent>();

            _inventorySlots = new MBList<InventorySlotVM>();
            for (int i = 0; i < inventoryCapacity; i++)
            {
                _inventorySlots.Add(new InventorySlotVM());
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();

            _client.OnMyRepresentativeAssigned -= OnMyRepresentativeAssigned;

            if (_isMyRepresentativeAssigned)
            {
                _client.MyRepresentative.OnPeerBountyUpdated -= OnPeerBountyUpdated;
            }
        }

        public void Tick(float dt)
        {
            _updatePlayersTimeElapsed += dt;
            if (_updatePlayersTimeElapsed >= UpdatePlayersDuration)
            {
                _updatePlayersTimeElapsed = 0.0f;
            }
        }

        private void OnMyRepresentativeAssigned()
        {
            //_client.MyRepresentative.OnPeerBountyUpdated += OnPeerBountyUpdated;

            _isMyRepresentativeAssigned = true;

        }

        private void OnPeerBountyUpdated(NetworkCommunicator peer, int newBounty)
        {
            //var updatedPlayer = Players.FirstOrDefault(p => p.Peer.GetNetworkPeer() == peer);
        }

        #region Properties
        
        [DataSourceProperty]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        public InventoryArmorPanelVM ArmorPanelVM {
            get { return _armorVM; }
            set
            {
                if (value != _armorVM)
                {
                    _armorVM = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        public InventoryWeaponPanelVM WeaponPanelVM
        {
            get { return _weaponVM; }
            set
            {
                if (value != _weaponVM)
                {
                    _weaponVM = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        #endregion
    }
}
