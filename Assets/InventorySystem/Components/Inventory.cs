using Assets.InventorySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public int SlotsX, SlotsY;
    public GUISkin Skin;

    public List<Slot> InventorySlots = new List<Slot>();

    private ItemDatabase _database;
    private bool _showInventory;
    private bool _showTooltip;
    private string _tooltip;

    void Start () {

        for(int i = 0; i < (SlotsX * SlotsY); i++)
        {
            InventorySlots.Add(new Slot() { ID = i, Item = null, StackSize = 0 });
        }

        _database = GameObject.FindGameObjectWithTag("Item Database").GetComponent<ItemDatabase>();

        AddItem(1000);
        AddItem(1000);
        AddItem(1000);
        AddItem(1000);
        AddItem(1000);
        AddItem(1001);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        AddItem(1002);
        RemoveItem(1);
        RemoveItem(1);
        RemoveItem(1);
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
        {
            _showInventory = !_showInventory;
        }

        //Debug.Log("Contains: " + InventoryContains(1000, 5));
        //MoveItem(2, 1);
    }

    void OnGUI()
    {
        _tooltip = string.Empty;

        GUI.skin = Skin;
        if(_showInventory)
        {
            DrawInventory();
        }

        if(_showTooltip)
        {
            GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 200, 200), _tooltip);
        }
    }

    void DrawInventory()
    {
        int slotId = 0;
        int slotSize = 50;
        int padding = 5;

        for (int y = 0; y < SlotsY; y++)
        {
            for (int x = 0; x < SlotsX; x++)
            {
                int xOffset = x * (slotSize + padding);
                int yOffset = y * (slotSize + padding);

                var slotRect = new Rect(xOffset, yOffset, slotSize, slotSize);
                GUI.Box(slotRect, string.Empty, Skin.GetStyle("Slot"));

                var slot = InventorySlots[slotId];
                if (slot.Item != null)
                {
                    GUI.DrawTexture(slotRect, slot.Item.Icon);
                    GUI.Label(slotRect, slot.StackSize.ToString());
                    
                    if (slotRect.Contains(Event.current.mousePosition))
                    {
                        CreateTooltip(slot.Item);
                        _showTooltip = true;
                    }
                }

                if(string.IsNullOrEmpty(_tooltip))
                {
                    _showTooltip = false;
                }
                

                slotId++;
            }
        }
    }

    void CreateTooltip(Item item)
    {
        _tooltip = item.Name;
    }

    void AddItem(int itemId)
    {
        var item = _database.Items.First(it => it.ID == itemId);

        if(item.MaxStackSize == 1)
        {
            // item not stackable
            AddToFirstAvailableSlot(item);
            return;
        }

        var existingSlotsWithItem =
            InventorySlots.Where(slot => slot.Item != null && slot.Item.ID == item.ID);

        if (!existingSlotsWithItem.Any())
        {
            // No slots contain this item to append to.
            AddToFirstAvailableSlot(item);
        }
        else
        {
            // Some slots contain this item.
            // Attempt to stack this item in an existing slot.
            var stackable = false;
            foreach (var slot in existingSlotsWithItem)
            {
                stackable = slot.StackSize + 1 <= slot.Item.MaxStackSize;

                if (stackable)
                {
                    slot.Add(item);
                    break;
                }
            }

            // Could not stack on any of the existing slots.
            if (!stackable) AddToFirstAvailableSlot(item);
        }
        
    }

    void AddToFirstAvailableSlot(Item item)
    {
        foreach (var slot in InventorySlots)
        {
            if (slot.Item == null)
            {
                slot.Add(item);
                break;
            }
        }
    }

    void RemoveItem(int slotId)
    {
        var slot = InventorySlots.Find(s => s.ID == slotId);
        Debug.Log("Slot: " + slot.ID);
        slot.Remove();
    }

    void MoveItem(int fromSlotId, int toSlotId)
    {
        if (InventorySlots[fromSlotId] == null) return;

        var movingItem = InventorySlots[fromSlotId];
        var targetItem = InventorySlots[toSlotId];

        InventorySlots[toSlotId] = movingItem;
        InventorySlots[fromSlotId] = targetItem;
    }

    bool InventoryContains(int itemId, int amount)
    {
        var numItems = InventorySlots
            .Where(s => s.Item != null && s.Item.ID == itemId)
            .Select(s => s.StackSize)
            .Aggregate((a, b) => a + b);

        return numItems >= amount;
    }
}
