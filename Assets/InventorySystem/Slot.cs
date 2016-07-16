using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.InventorySystem
{
    [Serializable]
    public class Slot
    {
        public int ID;
        public Item Item;
        public int StackSize;

        public bool Add(Item item)
        {
            if(StackSize + 1 <= item.MaxStackSize)
            {
                Item = item;
                StackSize++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Remove()
        {
            Item = null;
            StackSize = 0;
        }
    }
}
