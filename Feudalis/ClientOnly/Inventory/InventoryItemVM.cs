using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using static TaleWorlds.Core.ItemObject;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryItemVM : ViewModel
    {

        private ItemObject _itemObject;
        private EquipmentElement? _equipmentElement;
        private MissionWeapon? _missionWeapon;
        private Boolean _isEquipped;

        public InventoryItemVM()
        {

        }

        public InventoryItemVM(MissionWeapon weapon, bool isEquipped)
        {
            _missionWeapon = weapon;
            _itemObject = weapon.Item;
        }

        public InventoryItemVM(EquipmentElement equipment, bool isEquipped)
        {
            _equipmentElement = equipment;
            _itemObject = equipment.Item;
        }

        public Boolean IsWeapon()
        {
            return _missionWeapon != null;
        }

        public Boolean IsArmor()
        {
            return _equipmentElement != null;
        }

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

        public EquipmentElement? Equipment
        {
            get
            {
                return this._equipmentElement;
            }
            set
            {
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
