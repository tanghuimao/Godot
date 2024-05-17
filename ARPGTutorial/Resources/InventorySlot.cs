using Godot;
using System;

/// <summary>
/// 背包槽
/// </summary>
[GlobalClass]
[GodotClassName("InventorySlot")]
public partial class InventorySlot : Resource
{
    //物品
    public InventoryItem InventoryItem;
    //当前数量
    public short Amount;
    //最大数量
    [Export] public short MaxAmount = 10;
}