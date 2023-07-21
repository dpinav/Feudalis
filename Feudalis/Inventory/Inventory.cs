using System.Collections.Generic;
using TaleWorlds.Core;

namespace Feudalis.Inventory
{
    public class Inventory
    {
        public readonly List<InventorySlot> Storage;

        public Inventory()
        {
            Storage = new List<InventorySlot>(30);

            for (int i = 0; i < Storage.Capacity; i++) Storage.Add(new InventorySlot());
        }


        public bool AddItemToStorage(ItemObject item, int centralStorageIndex)
        {
            if (IsValidIndex(centralStorageIndex) && IsSlotEmpty(centralStorageIndex))
            {
                Storage[centralStorageIndex].Item = item;
                return true;
            }

            return false;
        }

        private bool IsSlotEmpty(int centralStorageIndex)
        {
            return Storage[centralStorageIndex].Item == null;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < Storage.Count;
        }
    }
}
