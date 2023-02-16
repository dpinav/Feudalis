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
        private bool _isEnabled;

        private MBBindingList<InventorySlotVM> _inventorySlots;
        private InventoryArmorPanelVM _armorVM;
        private InventoryWeaponPanelVM _weaponVM;

        private const float UpdatePlayersDuration = 5.0f;
        private float _updatePlayersTimeElapsed = 0.0f;

        public InventoryVM(MissionMultiplayerFeudalisClient client, int inventoryCapacity)
        {
            _client = client;
            _client.OnMyRepresentativeAssigned += OnMyRepresentativeAssigned;
            _gameMode = Mission.Current.GetMissionBehavior<MissionMultiplayerGameModeBaseClient>();

            _armorVM = new InventoryArmorPanelVM(_client.MyRepresentative);
            _weaponVM = new InventoryWeaponPanelVM(_client.MyRepresentative);
            _inventorySlots = new MBBindingList<InventorySlotVM>();
            for (int i = 0; i < inventoryCapacity; i++)
            {
                _inventorySlots.Add(new InventorySlotVM(InventorySlotType.StorageSlot));
            }
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            _armorVM.RefreshValues();
            _weaponVM.RefreshValues();
            foreach (var slot in _inventorySlots) slot.RefreshValues();
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

        public MBBindingList<InventorySlotVM> InventorySlots
        {
            get { return _inventorySlots; }
            set
            {
                if (value != _inventorySlots)
                {
                    _inventorySlots = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        #endregion
    }
}
