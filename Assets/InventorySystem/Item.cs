using System;
using UnityEngine;
using System.Collections;
using Assets.InventorySystem.Enums;

namespace Assets.InventorySystem
{
    [Serializable]
    public class Item
    {
        public Item()
        {

        }

        public Item(string name, int id, string desc, int power, int speed, ItemType type, int maxStackSize)
        {
            Name = name;
            ID = id;
            Desc = desc;
            Icon = Resources.Load<Texture2D>("ItemIcons/" + name);
            Type = type;
            MaxStackSize = maxStackSize == 0 ? 1 : maxStackSize;
        }

        public string Name;
        public int ID;
        public string Desc;
        public Texture2D Icon;
        public ItemType Type;
        public int MaxStackSize;
    }
}

