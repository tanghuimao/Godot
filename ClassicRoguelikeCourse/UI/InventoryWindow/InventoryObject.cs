using System;
using Godot;

namespace ClassicRoguelikeCourse.UI.InventoryWindow;
/// <summary>
/// 背包物品
/// </summary>
public partial class InventoryObject : Button, IUi
{
    //选择事件
    public event Action<InventoryObject> Selected; 
    public void Initialize()
    {
        // 焦点获取事件
        FocusEntered += () => Selected?.Invoke(this);
    }

    public void Update(double delta)
    {
    }
}