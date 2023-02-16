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
    public class InventoryWeaponPanelVM : ViewModel
    {
        private FeudalisMissionRepresentative _representative;
        private InventorySlotVM _weapon0Slot;
        private InventorySlotVM _weapon1Slot;
        private InventorySlotVM _weapon2Slot;
        private InventorySlotVM _weapon3Slot;

        public InventoryWeaponPanelVM(FeudalisMissionRepresentative representative)
        {
            _representative = representative;
            _weapon0Slot = new InventorySlotVM(InventorySlotType.WeaponSlot);
            _weapon1Slot = new InventorySlotVM(InventorySlotType.WeaponSlot);
            _weapon2Slot = new InventorySlotVM(InventorySlotType.WeaponSlot);
            _weapon3Slot = new InventorySlotVM(InventorySlotType.WeaponSlot);
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            Agent agent = _representative.ControlledAgent;
            _weapon0Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon0], true);
            _weapon1Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon1], true);
            _weapon2Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon2], true);
            _weapon3Slot.InventoryItem = new InventoryItemVM(agent.Equipment[(int)EquipmentIndex.Weapon3], true);
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
