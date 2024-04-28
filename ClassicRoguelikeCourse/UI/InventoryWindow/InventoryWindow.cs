using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.PickableObjects.Equipments;
using ClassicRoguelikeCourse.Entities.PickableObjects.Items;
using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.UI.InventoryWindow;
/// <summary>
/// 背包窗口
/// </summary>
public partial class InventoryWindow : CanvasLayer, IUi
{
    //背包物品预设
    private PackedScene _inventoryObjectScene;
    //背包物品描述
    private  Label _descriptionLabel;
    //选择物品索引
    private int _selectedIndex = -1;
    //背包容器
    private VBoxContainer _inventoryContainer;
    //玩家
    private  Player _player;
    public void Initialize()
    {
        //加载背包物品预设
        _inventoryObjectScene = GD.Load<PackedScene>("res://UI/InventoryWindow/InventoryObject.tscn");
        _descriptionLabel = GetNode<Label>("%DescriptionLabel");
        _inventoryContainer = GetNode<VBoxContainer>("%InventoryContainer");
        _player = GetTree().CurrentScene.GetNode<Player>("%Player");
    }

    public void Update(double delta)
    {
    }
    /// <summary>
    /// 切换背包窗口
    /// </summary>
    public void Toggle()
    {
        //切换显示
        Visible = !Visible;
        //如果显示
        if (Visible)
        {
            GenerateInventoryObjects();
        }
        else
        {
            ClearInventoryObjects();
        }
    }
    /// <summary>
    /// 使用背包物品
    /// </summary>
    public void UseInventoryObject()
    {
        //玩家数据
        var playerData = _player.CharacterData as PlayerData;
        //如果背包为空
        if (playerData.Inventory.Count == 0) return;
        //获取背包物品
        var pickableObject = playerData.Inventory[_selectedIndex];
        //如果不是使用效果物品并且不是装备
        if (pickableObject is not IConsumableItem && pickableObject is not Equipment) return;
        
        
        switch (pickableObject)
        {
            case IConsumableItem consumableItem:
                //使用物品
                consumableItem.Consume();
                break;
            case Equipment equipment:
                //装备物品
                equipment.Equip();
                break;
        }
        
        //关闭背包窗口
        Toggle();
    }

    /// <summary>
    /// 丢弃背包物品
    /// </summary>
    public void PutAwayInventoryObject()
    {
        //玩家数据
        var playerData = _player.CharacterData as PlayerData;
        //如果背包为空
        if (playerData.Inventory.Count == 0) return;
        //获取背包物品
        var pickableObject = playerData.Inventory[_selectedIndex];
        
        switch (pickableObject)
        {
            case IImmediateEffectItem immediateEffectItem:
                //卸载物品效果
                immediateEffectItem.UnDoImmediateEffect();
                break;
            case Equipment equipment:
                //卸载装备
                equipment.UnEquip();
                break;
        }
        //移除背包物品
        playerData.Inventory.Remove(pickableObject);
        //关闭背包窗口
        Toggle();
    }
    
    /// <summary>
    /// 生成背包物品
    /// </summary>
    private void GenerateInventoryObjects()
    {
        //玩家数据
        var playerData = _player.CharacterData as PlayerData;
        //如果背包为空
        if (playerData.Inventory.Count == 0) return;
        //遍历背包
        for (int i = 0; i < playerData.Inventory.Count; i++)
        {
            //生成背包物品
            var inventoryObject = _inventoryObjectScene.Instantiate<InventoryObject>();
            inventoryObject.Text = (i + 1) + ". " + playerData.Inventory[i].Name;
            
            //显示玩家装备
            if (playerData.Inventory[i] is Equipment && (playerData.Inventory[i] as Equipment).IsEquipped)
            {
                //显示已装备
                inventoryObject.Text += " [已装备]";
            }
            
            //添加背包物品到背包
            _inventoryContainer.AddChild(inventoryObject);
            //初始化背包物品
            inventoryObject.Initialize();
            //订阅事件
            inventoryObject.Selected += OnInventoryObjectSelected;
        }
        //聚焦背包第一项
        _selectedIndex = 0;
        _inventoryContainer.GetChild<Button>(_selectedIndex).GrabFocus();
    }

    /// <summary>
    /// 清理背包物品
    /// </summary>
    private void ClearInventoryObjects()
    {
        foreach (var child in _inventoryContainer.GetChildren())
        {
            child.QueueFree();
        }
        //清空描述
        _descriptionLabel.Text = string.Empty;
        _selectedIndex = -1;
    }
    /// <summary>
    /// 背包物品选中
    /// </summary>
    /// <param name="focusedInventoryObject">选中物品</param>
    private void OnInventoryObjectSelected(InventoryObject focusedInventoryObject)
    {
        //玩家数据
        var playerData = _player.CharacterData as PlayerData;
        //如果背包为空
        if (playerData.Inventory.Count == 0) return;
        //获取选中物品索引
        _selectedIndex = focusedInventoryObject.GetIndex();
        //获取选中物品
        var pickableObject = playerData.Inventory[_selectedIndex];
        //显示物品描述
        _descriptionLabel.Text = pickableObject.Description;
    }
}