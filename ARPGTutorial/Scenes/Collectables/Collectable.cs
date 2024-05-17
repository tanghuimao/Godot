using Godot;
using System;
/// <summary>
/// 可搜集物品
/// </summary>
public partial class Collectable : StaticBody2D
{
    /// <summary>
    /// 物品资源
    /// </summary>
    [Export] public InventoryItem InventoryItem;
    
    /// <summary>
    /// 搜集
    /// </summary>
    public virtual void Collect(Inventory inventory)
    {
        inventory.AddItem(InventoryItem);
        QueueFree();
    }
}
