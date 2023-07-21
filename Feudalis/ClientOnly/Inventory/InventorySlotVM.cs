using Feudalis.Inventory;
using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventorySlotVM : ViewModel
    {
        private InventoryItemVM _inventoryItem;
        private ImageIdentifierVM _imageIdentifier;
        private InventoryMissionRepresentative _inventoryRepresentative;

        public int Index;
        public EquipmentIndex EquipmentIndex;


        public InventorySlotVM(int index, EquipmentIndex equipmentIndex)
        {
            _inventoryRepresentative = InventoryMissionRepresentative.GetInventoryRepresentative();
            _inventoryItem = new InventoryItemVM();
            _imageIdentifier = new ImageIdentifierVM();

            EquipmentIndex = equipmentIndex;
            Index = index;
        }

        public InventorySlotVM(int index, InventoryItemVM inventoryItem, EquipmentIndex equipmentIndex) : this(index, equipmentIndex)
        {
            _inventoryItem = inventoryItem;
            _imageIdentifier = _inventoryItem?.ItemObject != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            //ImageIdentifier = _inventoryItem?.ItemObject != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM();
        }

        public void ExecuteTransferToSlot(InventorySlotVM draggedSlot, int index)
        {
            InventorySlotVM targetSlot = this;
            /*
            if (draggedSlot.IsEmpty() || !targetSlot.IsEmpty())
            {
                FeudalisChatLib.ChatMessage("Return ", Colors.Magenta);
                return;
            }*/

            if (targetSlot.EquipmentIndex == EquipmentIndex.None)
            {
                _inventoryRepresentative.MoveItemToStorage(
                    targetSlot.Index, targetSlot.GetPanelIndex(), draggedSlot.Index, draggedSlot.GetPanelIndex()
                    );
            }
        }

        public Boolean IsEmpty()
        {
            return _inventoryItem == null;
        }

        public int GetPanelIndex()
        {
            if (this.EquipmentIndex == EquipmentIndex.None)
            {
                return (int)PanelIndex.StoragePanel;
            }
            if (this.EquipmentIndex > EquipmentIndex.ExtraWeaponSlot)
            {
                return (int)PanelIndex.ArmorPanel;
            }
            if (this.EquipmentIndex < EquipmentIndex.ExtraWeaponSlot)
            {
                return (int)PanelIndex.WeaponPanel;
            }

            return 0;
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

    public enum PanelIndex
    {
        StoragePanel = 0,
        ArmorPanel = 1,
        WeaponPanel = 2,
    }
}
