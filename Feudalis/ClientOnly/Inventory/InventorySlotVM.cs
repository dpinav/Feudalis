using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventorySlotVM : ViewModel
    {
        private InventoryItemVM _inventoryItem;


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
            get => _inventoryItem != null ? new ImageIdentifierVM(_inventoryItem.ItemObject) : new ImageIdentifierVM();
        }
        #endregion

    }
}
