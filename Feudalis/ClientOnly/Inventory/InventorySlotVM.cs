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
    public class InventorySlotVM : ViewModel
    {
        private InventoryItemVM _inventoryItem;
        private EquipmentIndex _index;
        private ImageIdentifierVM _imageIdentifier;

        
        public InventorySlotVM(EquipmentIndex index)
        {
            _inventoryItem = new InventoryItemVM();
            _index = index;
            _imageIdentifier = new ImageIdentifierVM();
        }

        public InventorySlotVM(InventoryItemVM inventoryItem, EquipmentIndex index) : this(index)
        {
            _inventoryItem = inventoryItem;
            _imageIdentifier = _inventoryItem?.ItemObject != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            //ImageIdentifier = _inventoryItem?.ItemObject != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM();
        }

        public void ExecuteTransferItem(InventorySlotVM draggedSlot, int index)
        {
            InventorySlotVM targetSlot = this;

            if (draggedSlot.IsEmpty() || !targetSlot.IsEmpty())
            {
                return;
            }

            if (targetSlot._index == EquipmentIndex.None)
            {

            }
        }

        public Boolean IsEmpty()
        {
            return _inventoryItem == null;
        }


        #region Properties
        [DataSourceProperty]
        public InventoryItemVM InventoryItem
        {
            get { return _inventoryItem; }
            set
            {
                if (value != _inventoryItem)
                {
                    _inventoryItem = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM ImageIdentifier
        {
            get { return _inventoryItem?.ItemObject != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM(); }
            set
            {
                if (value != _imageIdentifier)
                {
                    _imageIdentifier = value;
                    OnPropertyChangedWithValue(value);
                }
            }
        }
        #endregion

    }

    public enum InventorySlotType
    {
        StorageSlot = 0,
        ArmorSlot = 1,
        WeaponSlot = 2,
    }
}
