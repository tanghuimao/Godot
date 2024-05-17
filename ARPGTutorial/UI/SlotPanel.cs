using Godot;
using System;
/// <summary>
/// 物品槽
/// </summary>
public partial class SlotPanel : Button
{
    //中心容器
    private CenterContainer _centerContainer;
    //物品面板
    public ItemPanel _itemPanel;
    //索引
    public int Index;
    
    public override void _Ready()
    {
        _centerContainer = GetNode<CenterContainer>("CenterContainer");
    }
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <param name="itemPanel"></param>
    public void Insert(ItemPanel itemPanel)
    {
        _itemPanel = itemPanel;
        _centerContainer.AddChild(_itemPanel);
    }
    /// <summary>
    /// 获取物品
    /// </summary>
    public ItemPanel TakeItem()
    {
        var itemPanel = _itemPanel;
        _centerContainer.RemoveChild(_itemPanel);
        _itemPanel = null;
        return itemPanel;
    }
    /// <summary>
    /// 判断是否为空
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return _itemPanel == null;
    }
}
