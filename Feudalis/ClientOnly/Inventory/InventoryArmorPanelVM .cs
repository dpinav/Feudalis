using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryArmorPanelVM : ViewModel
    {

        private FeudalisMissionRepresentative _representative;
        private InventorySlotVM _headSlot;
        private InventorySlotVM _shoulderSlot;
        private InventorySlotVM _handSlot;
        private InventorySlotVM _bodySlot;
        private InventorySlotVM _legSlot;

        // Might be better to pass responsability of creating the InventoryItems to the panels...
        public InventoryArmorPanelVM(FeudalisMissionRepresentative representative)
        {
            _representative = representative;
            _headSlot = new InventorySlotVM(InventorySlotType.ArmorSlot);
            _shoulderSlot = new InventorySlotVM(InventorySlotType.ArmorSlot);
            _handSlot = new InventorySlotVM(InventorySlotType.ArmorSlot);
            _bodySlot = new InventorySlotVM(InventorySlotType.ArmorSlot);
            _legSlot = new InventorySlotVM(InventorySlotType.ArmorSlot);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Agent agent = _representative.ControlledAgent;
            _headSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Head], true);
            _shoulderSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Cape], true);
            _handSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Gloves], true);
            _bodySlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Body], true);
            _legSlot.InventoryItem = new InventoryItemVM(agent.SpawnEquipment[(int)EquipmentIndex.Leg], true);
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
