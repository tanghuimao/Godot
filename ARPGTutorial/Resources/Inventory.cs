using Godot;
using System;
using System.Linq;
using Godot.Collections;

/// <summary>
/// 背包容器资源
/// </summary>
[GlobalClass]
[GodotClassName("Inventory")]
public partial class Inventory : Resource
{
    //背包变化事件
    public event Action InventoryChangedEvent;
    
    //背包物品集合
    [Export] public Array<InventorySlot> Slots = new ();
    /// <summary>
    /// 新增物品
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(InventoryItem item)
    {
        var inventorySlot = Slots.FirstOrDefault(slot => slot.InventoryItem?.Id == item.Id 
                                                         && slot.Amount < slot.MaxAmount);
        if (inventorySlot != null)
        {
            if (inventorySlot.Amount < inventorySlot.MaxAmount)
            {
                inventorySlot.InventoryItem = item;
                inventorySlot.Amount++;
            }
            else
            {
                CreateSlot(item);
            }
        }
        else
        {
            CreateSlot(item);
        }
        InventoryChangedEvent?.Invoke();
    }

    private void CreateSlot(InventoryItem item)
    {
        var inventorySlot = Slots.FirstOrDefault(slot => slot.InventoryItem == null);
        if (inventorySlot == null) return;
        inventorySlot.InventoryItem = item;
        inventorySlot.Amount++;
    }
    
    public void RemoveItemByIndex(int index)
    {
        Slots[index] = new InventorySlot();
    }

    public void InsertItemByIndex(int index, InventorySlot slot)
    {
        var oldIndex = Slots.IndexOf(slot);
        RemoveItemByIndex(oldIndex);
        Slots[index] = slot;
    }
}
