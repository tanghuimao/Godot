using Godot;
using System;
using System.Linq;
using Godot.Collections;

/// <summary>
/// 背包
/// </summary>
public partial class InventoryControl : Control
{
    //背包事件
    public event Action<bool> OpenOrCloseEvent;
    //玩家
    private Player _player;
    //背包数据
    private Array<SlotPanel> _slotPanels = new();
    //当前选中背包槽
    private ItemPanel _currentItem;

    public override void _Ready()
    {
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
        //背包数据改变
        _player.Inventory.InventoryChangedEvent += () => Update();
        var children = GetNode<GridContainer>("%GridContainer").GetChildren();
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is SlotPanel slotPanel)
            {
                _slotPanels.Add(slotPanel);
                slotPanel.Index = i;
                slotPanel.Pressed += () =>
                {
                    if (slotPanel.IsEmpty() && _currentItem != null)
                    {
                        InsertItem(slotPanel);
                        return;
                    }
                    if (!slotPanel.IsEmpty())
                    {
                        _currentItem = slotPanel.TakeItem();
                        AddChild(_currentItem);
                        UpdateItemPanelPosition();
                    }
                };
            }
        }
        Update();
    }
    /// <summary>
    /// 输入
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        UpdateItemPanelPosition();
    }
    /// <summary>
    /// 插入物品
    /// </summary>
    /// <param name="slotPanel"></param>
    public void InsertItem(SlotPanel slotPanel)
    {
        var item = _currentItem;
        RemoveChild(_currentItem);
        _currentItem = null;
        slotPanel.Insert(item);
    }
    
    /// <summary>
    /// 更新背包物品位置
    /// </summary>
    public void UpdateItemPanelPosition()
    {
        if (_currentItem != null)
        {
            //设置全局位置
            _currentItem.GlobalPosition = GetGlobalMousePosition() - _currentItem.Size / 2;
        }
    }

    /// <summary>
    /// 打开或关闭背包
    /// </summary>
    public void OpenOrClose()
    {
        Visible = !Visible;
        OpenOrCloseEvent?.Invoke(Visible);
    }
    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        var inventoryItemsCount = _player.Inventory.Slots.Count;
        var slotPanelsCount = _slotPanels.Count;
        var min = Math.Min(inventoryItemsCount, slotPanelsCount);
        
        for (var i = 0; i < min; i++)
        {
            //背包槽
            var inventorySlot = _player.Inventory.Slots[i];
            //背包槽面板
            var itemPanel = _slotPanels[i]._itemPanel;
            if (itemPanel == null)
            {
                var instantiate = GD.Load<PackedScene>("res://UI/ItemPanel.tscn").Instantiate();
                if (instantiate is ItemPanel item)
                {
                    itemPanel = item;
                }
                _slotPanels[i].Insert(itemPanel);
            };
            itemPanel.Update(inventorySlot);
            
        }
    }
}
