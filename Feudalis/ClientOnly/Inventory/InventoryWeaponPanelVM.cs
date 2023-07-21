using Feudalis.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryWeaponPanelVM : ViewModel
    {
        public NetworkCommunicator PlayerNetwork { get; private set; }
        public MissionPeer Player { get; private set; }
        public bool HasPeerAndMissionComponents { get; private set; } = false;

        private InventoryMissionView _missionView;
        private InventoryMissionRepresentative _inventoryRepresentative;

        private InventorySlotVM _weapon0Slot;
        private InventorySlotVM _weapon1Slot;
        private InventorySlotVM _weapon2Slot;
        private InventorySlotVM _weapon3Slot;

        public InventoryWeaponPanelVM(InventoryMissionView view)
        {
            _missionView = view;

            NetworkCommunicator peer = GameNetwork.MyPeer;
            _inventoryRepresentative = InventoryMissionRepresentative.GetInventoryRepresentative(peer);

            _weapon0Slot = new InventorySlotVM(0, EquipmentIndex.Weapon0);
            _weapon1Slot = new InventorySlotVM(1, EquipmentIndex.Weapon1);
            _weapon2Slot = new InventorySlotVM(2, EquipmentIndex.Weapon2);
            _weapon3Slot = new InventorySlotVM(3, EquipmentIndex.Weapon3);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();

            if (!HasPeerAndMissionComponents)
            {
                TryGetPeerAndMissionComponents();
            }
            Agent agent = this.Player.ControlledAgent;
            if (agent == null)
            {
                return;
            }

            _weapon0Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon0], true);
            _weapon1Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon1], true);
            _weapon2Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon2], true);
            _weapon3Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon3], true);
        }

        private void TryGetPeerAndMissionComponents()
        {
            this.PlayerNetwork = GameNetwork.MyPeer;
            this.Player = this.PlayerNetwork.GetComponent<MissionPeer>();
            if (this.Player == null)
                return;

            _inventoryRepresentative = InventoryMissionRepresentative.GetInventoryRepresentative(this.PlayerNetwork);
            if (_inventoryRepresentative == null)
                return;
            this.HasPeerAndMissionComponents = true;
        }

        #region Properties

        [DataSourceProperty]
        public InventorySlotVM Weapon0Slot
        {
            get { return _weapon0Slot; }
            set
            {
                if (value != _weapon0Slot)
                {
                    _weapon0Slot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM Weapon1Slot
        {
            get { return _weapon1Slot; }
            set
            {
                if (value != _weapon1Slot)
                {
                    _weapon1Slot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM Weapon2Slot
        {
            get { return _weapon2Slot; }
            set
            {
                if (value != _weapon2Slot)
                {
                    _weapon2Slot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM Weapon3Slot
        {
            get { return _weapon3Slot; }
            set
            {
                if (value != _weapon3Slot)
                {
                    _weapon3Slot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        #endregion
    }
}
