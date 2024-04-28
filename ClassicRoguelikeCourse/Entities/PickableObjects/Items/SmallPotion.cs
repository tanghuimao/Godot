using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;

/// <summary>
/// 小血瓶
/// </summary>
public partial class SmallPotion : Item, IConsumableItem
{
    //增加血量
    [Export] private float _healthIncrement = 10f;

    public override void Initialize()
    {
        base.Initialize();
        _description = "使用后增加" + _healthIncrement + "点血量";
    }

    public void Consume()
    {
        //设置玩家血量
        _player.CharacterData.Health += _healthIncrement;
        _player.CharacterData.Health = _player.CharacterData.Health >= _player.CharacterData.MaxHealth
            ? _player.CharacterData.MaxHealth
            : _player.CharacterData.Health;
        //移除使用物品
        (_player.CharacterData as PlayerData).Inventory.Remove(this);
    }
}