using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.Core.ItemObject;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryItemVM : ViewModel
    {

        private ItemObject _itemObject;
        private EquipmentElement _equipmentElement;

        #region Properties

        public ItemObject ItemObject
        {
            get => this._itemObject;
            set
            {
                if (value == this._itemObject)
                    return;
                this._itemObject = value;
                this.OnPropertyChangedWithValue((object)value, nameof(ItemObject));
            }
        }

        public EquipmentElement Equipment
        {
            get
            {
                return this._equipmentElement;
            }
            set
            {
                if (value.IsEqualTo(this._equipmentElement))
                    return;
                this._equipmentElement = value;
                this.OnPropertyChangedWithValue((object)value, nameof(Equipment));
            }
        }

        [DataSourceProperty]
        public ItemTypeEnum TypeId
        {
            get => _itemObject?.Type ?? 0;
        }
        #endregion
    }
}
