using Godot;
using System;
/// <summary>
/// 物品面板
/// </summary>
public partial class ItemPanel : Panel
{
    private Sprite2D _item;
    private Label _label;
    
    public override void _Ready()
    {
        _item = GetNode<Sprite2D>("Item");
        _label = GetNode<Label>("Label");
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="inventoryItem"></param>
    public void Update(InventorySlot slot)
    {
        if (slot.InventoryItem != null)
        {
            _item.Visible = true;
            _item.Texture = slot.InventoryItem.Texture;
            _label.Visible = true;
            _label.Text = slot.Amount.ToString();
        }
        else
        {
            _label.Visible = false;
        }
    }
}
