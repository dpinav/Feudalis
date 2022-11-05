using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace Feudalis.ClientOnly.Inventory
{
    public class InventoryArmorPanelVM : ViewModel
    {

        private InventorySlotVM _headSlot;
        private InventorySlotVM _shoulderSlot;
        private InventorySlotVM _handSlot;
        private InventorySlotVM _bodySlot;
        private InventorySlotVM _legSlot;

        public InventoryArmorPanelVM()
        {

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
