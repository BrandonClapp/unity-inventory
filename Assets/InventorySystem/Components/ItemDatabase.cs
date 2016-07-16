using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.InventorySystem;
using Assets.InventorySystem.Enums;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> Items = new List<Item>();

    void Start()
    {
        Items.AddRange(GetItems());
    }

    private List<Item> GetItems()
    {
        return new List<Item>
        {
            new Item("Hatchet", 1000, "A small hatchet.", 1, 1, ItemType.Weapon, 3),
            new Item("Pick Axe", 1001, "A small pick axe.", 1, 1, ItemType.Weapon, 1),
            new Item("Strawberry", 1003, "A juicy red strawberry", 0, 0, ItemType.Consumable, 5)
        };

    }
}
