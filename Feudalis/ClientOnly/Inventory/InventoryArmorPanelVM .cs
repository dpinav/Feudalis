using Feudalis.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryArmorPanelVM : ViewModel
    {

        public NetworkCommunicator PlayerNetwork { get; private set; }
        public MissionPeer Player { get; private set; }
        public bool HasPeerAndMissionComponents { get; private set; } = false;

        private InventoryMissionView _missionView;
        private InventoryMissionRepresentative _inventoryRepresentative;

        private InventorySlotVM _headSlot;
        private InventorySlotVM _shoulderSlot;
        private InventorySlotVM _handSlot;
        private InventorySlotVM _bodySlot;
        private InventorySlotVM _legSlot;

        // Might be better to pass responsability of creating the InventoryItems to the panels...
        public InventoryArmorPanelVM(InventoryMissionView missionView)
        {
            _missionView = missionView;

            _headSlot = new InventorySlotVM(5, EquipmentIndex.Head);
            _shoulderSlot = new InventorySlotVM(9, EquipmentIndex.Cape);
            _handSlot = new InventorySlotVM(8, EquipmentIndex.Gloves);
            _bodySlot = new InventorySlotVM(6, EquipmentIndex.Body);
            _legSlot = new InventorySlotVM(7, EquipmentIndex.Leg);
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
            _headSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Head], true);
            _shoulderSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Cape], true);
            _handSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Gloves], true);
            _bodySlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Body], true);
            _legSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Leg], true);
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
        public InventorySlotVM HeadSlot
        {
            get { return _headSlot; }
            set
            {
                if (value != _headSlot)
                {
                    _headSlot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM ShoulderSlot
        {
            get { return _shoulderSlot; }
            set
            {
                if (value != _shoulderSlot)
                {
                    _shoulderSlot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM HandSlot
        {
            get { return _handSlot; }
            set
            {
                if (value != _handSlot)
                {
                    _handSlot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM BodySlot
        {
            get { return _bodySlot; }
            set
            {
                if (value != _bodySlot)
                {
                    _bodySlot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public InventorySlotVM LegSlot
        {
            get { return _legSlot; }
            set
            {
                if (value != _legSlot)
                {
                    _legSlot = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        #endregion
    }
}
