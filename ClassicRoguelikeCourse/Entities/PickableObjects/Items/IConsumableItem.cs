namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 主动使用物品
/// </summary>
public interface IConsumableItem
{
    /// <summary>
    /// 使用物品
    /// </summary>
    public void Consume();
}