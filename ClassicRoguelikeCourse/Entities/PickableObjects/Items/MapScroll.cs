using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 地图卷
/// </summary>
public partial class MapScroll : Item, IConsumableItem
{
    public override void Initialize()
    {
        base.Initialize();
        _description = "使用后消除本地图战争迷雾";
    }

    public void Consume()
    {
        //TODO 清理地图战争迷雾
        
        //移除使用物品
        (_player.CharacterData as PlayerData).Inventory.Remove(this);
    }
}