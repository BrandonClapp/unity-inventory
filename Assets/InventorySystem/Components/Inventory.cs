using Assets.InventorySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public int SlotsX, SlotsY;
    public GUISkin Skin;
    public List<Slot> InventorySlots = new List<Slot>();
    public int SlotSize = 50;

    private ItemDatabase _database;
    private bool _showInventory;
    private bool _showTooltip;
    private string _tooltip;

    private bool _isDraggingItem;
    private Slot _draggingSlot;

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
            GUI.Box(
                new Rect(Event.current.mousePosition.x + 15f, Event.current.mousePosition.y, 200, 200),
                _tooltip,
                Skin.GetStyle("Tooltip"));
        }

        if (_isDraggingItem)
        {
            GUI.DrawTexture(
                new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, SlotSize, SlotSize),
                _draggingSlot.Item.Icon);
        }
    }

    void DrawInventory()
    {
        int slotId = 0;
        int padding = 5;
        Event e = Event.current;

        for (int y = 0; y < SlotsY; y++)
        {
            for (int x = 0; x < SlotsX; x++)
            {
                int xOffset = x * (SlotSize + padding);
                int yOffset = y * (SlotSize + padding);

                var slotRect = new Rect(xOffset, yOffset, SlotSize, SlotSize);
                GUI.Box(slotRect, string.Empty, Skin.GetStyle("Slot"));

                var slot = InventorySlots[slotId];
                if (slot.Item != null)
                {
                    GUI.DrawTexture(slotRect, slot.Item.Icon);
                    GUI.Label(slotRect, slot.StackSize.ToString());
                    
                    if (slotRect.Contains(e.mousePosition))
                    {
                        CreateTooltip(slot.Item);
                        _showTooltip = true;

                        if (e.button == 0 && e.type == EventType.MouseDrag && !_isDraggingItem)
                        {
                            BeginDragging(slot);
                        }

                        if (e.type == EventType.MouseUp && _isDraggingItem)
                        {
                            // dragging an item to another slot where item exists. (swap)
                            SwapSlotItems(_draggingSlot.ID, slot.ID);
                            EndDragging();
                        }
                    }
                }
                else
                {
                    if (slotRect.Contains(e.mousePosition))
                    {
                        if (e.type == EventType.MouseUp && _isDraggingItem)
                        {
                            MoveToEmptySlot(_draggingSlot.ID, slot.ID);
                            EndDragging();
                        }
                    }    
                }

                if (string.IsNullOrEmpty(_tooltip))
                {
                    _showTooltip = false;
                }
                

                slotId++;
            }
        }
    }

    void CreateTooltip(Item item)
    {
        _tooltip += "<color=#4DA4BF>" + item.Name + "</color>\n\n";
        _tooltip += "<color=#f2f2f2>" + item.Desc + "</color>";
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

    void MoveToEmptySlot(int fromSlotId, int toSlotId)
    {
        Debug.Log("ToSLot:" + toSlotId);
        //todo: assert that to slot does not have item.
        var targetSlot = InventorySlots[toSlotId];

        targetSlot.Item = _draggingSlot.Item;
        targetSlot.StackSize = _draggingSlot.StackSize;

        var fromSlot = InventorySlots.First(s => s.ID == _draggingSlot.ID);
        fromSlot.Item = null;
        fromSlot.StackSize = 0;
    }

    void SwapSlotItems(int fromSlotId, int toSlotId)
    {
        // todo: assert that toslot id has item;
        var targetSlot = InventorySlots[toSlotId];
        var movingSlot = InventorySlots.First(s => s.ID == _draggingSlot.ID);

        movingSlot.Item = targetSlot.Item;
        movingSlot.StackSize = targetSlot.StackSize;

        targetSlot.Item = _draggingSlot.Item;
        targetSlot.StackSize = _draggingSlot.StackSize;
    }

    bool InventoryContains(int itemId, int amount)
    {
        var numItems = InventorySlots
            .Where(s => s.Item != null && s.Item.ID == itemId)
            .Select(s => s.StackSize)
            .Aggregate((a, b) => a + b);

        return numItems >= amount;
    }

    void BeginDragging(Slot slot)
    {
        _draggingSlot = new Slot { ID = slot.ID, Item = slot.Item, StackSize = slot.StackSize };
        slot.Item = null;
        _isDraggingItem = true;
    }

    void EndDragging()
    {
        _draggingSlot = null;
        _isDraggingItem = false;
    }
}
