using Godot;
using System;
/// <summary>
/// 画布
/// </summary>
public partial class CanvasLayer : Godot.CanvasLayer
{
    private InventoryControl _inventoryControl;
    
    public override void _Ready()
    {
        _inventoryControl = GetNode<InventoryControl>("InventoryControl");
        _inventoryControl.OpenOrClose();
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("open_inventory"))
        {
            _inventoryControl.OpenOrClose();
        }
    }
}
