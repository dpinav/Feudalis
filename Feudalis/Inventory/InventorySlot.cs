using System;
using TaleWorlds.Core;

namespace Feudalis.Inventory
{
    public class InventorySlot
    {
        public string Index { get; set; }
        public ItemObject Item { get; set; }


        public InventorySlot() { }

        public InventorySlot(String index, ItemObject item)
        {
            this.Index = index;
            this.Item = item;
        }

        internal bool IsEmpty()
        {
            return this.Item == null;
        }
    }
}
